public class TodoItemDTO
{
    public int Id { get; set; }
    public string? Lat { get; set; }
    public string? Lon { get; set; }
    public byte? Env { get; set; }
    public int? Aqi { get; set; }
    public DateTime? Datet { get; set; }
    public TodoItemDTO() { }
    public TodoItemDTO(Todo todoItem) => (Id, Lat, Lon, Env, Aqi, Datet) = (todoItem.Id, todoItem.Lat, todoItem.Lon, todoItem.Env, todoItem.Aqi, todoItem.Datet);
}