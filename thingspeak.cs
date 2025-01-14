public class SensorDataDTO
{
    public string? Mp25 { get; set; } // Campo para almacenar la respuesta completa
}

public class ThingSpeakResponse
{
    public List<Feed> Feeds { get; set; } = new List<Feed>();
}

public class Feed
{
    public string? CreatedAt { get; set; }
    public string? Field1 { get; set; }
    public string? Field3 { get; set; }
    public string? Field4 { get; set; }
    public string? Field5 { get; set; }
}