using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jartisan.Application.Ports;
using Jartisan.Domain.Models;

namespace Jartisan.Infrastructure.Implementations.Maven
{
    public class MavenApi : IDependencyResolver
    {
        private static readonly HttpClient _httpClient = new(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15) 
        })
        {
            DefaultRequestHeaders = { { "User-Agent", "Jartisan-CLI/1.0" } }
        };

        
        private record QueryParameters(string? GroupId, string? ArtifactId, string? Version, string? SearchText)
        {
            public bool IsExactMatch => !string.IsNullOrEmpty(GroupId) && !string.IsNullOrEmpty(ArtifactId);
            public bool HasVersion => !string.IsNullOrEmpty(Version);
        }

        public async Task<List<DependencyInfo>> ResolveAsync(string query, CancellationToken cancellationToken = default)
        {
            var results = new List<DependencyInfo>();

            if (string.IsNullOrWhiteSpace(query)) return results;

            // Transforms user input into structured parameters
            var parsed = ParseQuery(query);
            string solrQuery;

            if (parsed.IsExactMatch)
            {
                // g:"group" AND a:"artifact" (with or without version)
                solrQuery = parsed.HasVersion 
                    ? $"g:\"{parsed.GroupId}\" AND a:\"{parsed.ArtifactId}\" AND v:\"{parsed.Version}\""
                    : $"g:\"{parsed.GroupId}\" AND a:\"{parsed.ArtifactId}\"";
            }
            else if (!string.IsNullOrEmpty(parsed.SearchText) && parsed.HasVersion)
            {
                // If user typed free text + version (Ex: jackson : 2.15.2)
                // Broad search for the term, but strictly ties to the typed version
              solrQuery = $"a:\"{parsed.SearchText!}\" AND v:\"{parsed.Version}\"";
            }
            else
            {
                // Only pure free text (your previous versatile solution)
                solrQuery = $"{parsed.SearchText!} OR a:{parsed.SearchText!}^2";
            }
            var sanitizedQuery = Uri.EscapeDataString(solrQuery);
            var url = $"https://search.maven.org/solrsearch/select?q={sanitizedQuery}&rows=10&wt=json";

            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                
                using var doc = JsonDocument.Parse(json);
                
                if (!doc.RootElement.TryGetProperty("response", out var responseProp) ||
                    !responseProp.TryGetProperty("docs", out var docsProp))
                {
                    return results;
                }

                foreach (var item in docsProp.EnumerateArray())
                {
                    var groupId = item.TryGetProperty("g", out var g) ? g.GetString() : null;
                    var artifactId = item.TryGetProperty("a", out var a) ? a.GetString() : null;
                    
                    string version = "unknown";
                    if (item.TryGetProperty("v", out var v))
                    {
                        version = v.GetString() ?? "unknown";
                    }
                    else if (item.TryGetProperty("latestVersion", out var lv))
                    {
                        version = lv.GetString() ?? "unknown";
                    }

                    if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(artifactId))
                    {
                        results.Add(new DependencyInfo(groupId, artifactId, version));
                    }
                }

                return results;
            }
            catch (Exception ex)
            {   
                Console.WriteLine($"Erro ao buscar no Maven: {ex.Message}");
                return results; 
            }
        }

        private static QueryParameters ParseQuery(string query)
{
            if (string.IsNullOrWhiteSpace(query))
                return new QueryParameters(null, null, null, null);

          
            var tokens = query.Split(':', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Trim();
            }

            // If it's GroupId ArtifactId and the version (optional)
            if (tokens.Length >= 2 && tokens[0].Contains('.'))
            {
                var groupId = tokens[0];
                var artifactId = tokens[1];
                var version = tokens.Length >= 3 ? tokens[2] : null;

                return new QueryParameters(groupId, artifactId, version, null);
            }

            // If it's Free Text + Version
            if (tokens.Length == 2)
            {
                var searchText = tokens[0];
                var version = tokens[1];

                return new QueryParameters(null, null, version, searchText);
            }

            // Se for Apenas Texto Livre puro 
            return new QueryParameters(null, null, null, query.Trim());
        }

    }
}
