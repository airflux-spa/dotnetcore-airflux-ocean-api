// DTO para los datos del sensor
public class SensorDataDTO
{
    public DateTime Timestamp { get; set; }
    public double? Mp10 { get; set; }
    public double? Mp25 { get; set; }
}

// Clase para deserializar la respuesta de ThingSpeak
public class ThingSpeakResponse
{
    public Feed[] Feeds { get; set; } = Array.Empty<Feed>(); // Inicializar con un array vacío para evitar nulos
}

public class Feed
{
    public string? CreatedAt { get; set; }
    public string? Field4 { get; set; }
    public string? Field5 { get; set; }
}
