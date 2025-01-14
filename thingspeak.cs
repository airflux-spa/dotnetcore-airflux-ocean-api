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
    public DateTime? CreatedAt { get; set; }
    public int? Field1 { get; set; }
    public int? Field3 { get; set; }
    public int? Field4 { get; set; }
    public int? Field5 { get; set; }
}