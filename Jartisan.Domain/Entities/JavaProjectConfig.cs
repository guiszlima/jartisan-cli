namespace Jartisan.Domain.Entities
{
    public record JavaProjectConfig
    {
      
        public string GroupId { get; set; } = "com.jartisan";
        public string? ArtifactId { get; set; } = null; 
        public string Version { get; set; } = "1.0-SNAPSHOT";
    }
}
