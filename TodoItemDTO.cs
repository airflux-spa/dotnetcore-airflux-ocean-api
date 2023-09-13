public class TodoItemDTO
{
    public int Id { get; set; }
    public string? Lat { get; set; }
    public string Lon { get; set; }
    public int? Aqi { get; set; }

    public string? Datet { get; set; }
    public TodoItemDTO() { }
    public TodoItemDTO(Todo todoItem) => (Id, Lat, Lon, Aqi, Datet) = (todoItem.Id, todoItem.Lat, todoItem.Lon, todoItem.Aqi, todoItem.Datet);
}