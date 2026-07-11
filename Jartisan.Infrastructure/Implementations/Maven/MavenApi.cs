using System;
using System.Collections.Generic; // Necessário para List<T>
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

        // Mudamos o retorno para List<DependencyInfo>
        public async Task<List<DependencyInfo>> ResolveAsync(string query, CancellationToken cancellationToken = default)
        {
            var results = new List<DependencyInfo>();

            if (string.IsNullOrWhiteSpace(query)) return results;

            var sanitizedQuery = Uri.EscapeDataString(query);
            // Alterado para rows=10
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

                // Loop para processar todos os itens do array de resultados
                foreach (var item in docsProp.EnumerateArray())
                {
                    var groupId = item.TryGetProperty("g", out var g) ? g.GetString() : null;
                    var artifactId = item.TryGetProperty("a", out var a) ? a.GetString() : null;
                    var version = item.TryGetProperty("latestVersion", out var v) ? v.GetString() : null;

                    if (!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(artifactId))
                    {
                        results.Add(new DependencyInfo(groupId, artifactId, version ?? "unknown"));
                    }
                }

                return results;
            }
            catch (Exception ex)
            {   
                Console.WriteLine($"Erro ao buscar no Maven: {ex.Message}");
                return results; // Retorna lista vazia em caso de erro
            }
        }
    }
}