public class TodoItemDTO
{
    public int Id { get; set; }
    public string? Deviceid { get; set; }
    public string? Tskey { get; set; }
    public string? Tschannel { get; set; }
    public string? Apikey { get; set; }
    public string? Lat { get; set; }
    public string? Lon { get; set; }
    public byte? Env { get; set; }
    public byte? Priv { get; set; }
    public int? Tem { get; set; }
    public int? Pre { get; set; }
    public int? Hum { get; set; }
    public int? Pm10 { get; set; }
    public int? Pm25 { get; set; }
    public int? Riesgo { get; set; }
    public int? Aqi { get; set; }
    public DateTime? Datet { get; set; }
    public TodoItemDTO() { }
    public TodoItemDTO(Todo todoItem) => (Id, Deviceid, Tskey, Tschannel, Apikey, Lat, Lon, Env, Priv, Tem, Pre, Hum, Pm10, Pm25, Riesgo, Aqi, Datet) = 
    (todoItem.Id, todoItem.Deviceid, todoItem.Tskey, todoItem.Tschannel, todoItem.Apikey , todoItem.Lat, todoItem.Lon, todoItem.Env, todoItem.Priv, todoItem.Tem, todoItem.Pre, todoItem.Hum, todoItem.Pm10, todoItem.Pm25, todoItem.Riesgo, todoItem.Aqi, todoItem.Datet);
}