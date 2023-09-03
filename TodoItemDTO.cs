public class TodoItemDTO
{
    public int Id { get; set; }
    public string? lat { get; set; }
    public string lon { get; set; }
    public int? aqi { get; set; }

    public TodoItemDTO() { }
    public TodoItemDTO(Todo todoItem) =>
    (Id, lat, lon, aqi) = (todoItem.Id, todoItem.lat, todoItem.lon, todoItem.aqi);
}