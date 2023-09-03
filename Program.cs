using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

app.Run();

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
}


static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
            ? TypedResults.Ok(new TodoItemDTO(todo))
            : TypedResults.NotFound();
}


static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
{
    var todo = await db.Todos.FindAsync(todoItemDTO.Id);

    if (todo is null)
    {
        var todoItem = new Todo
        {
            Id = todoItemDTO.Id,
            lat = todoItemDTO.lat,
            lon = todoItemDTO.lon,
            aqi = todoItemDTO.aqi
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        todoItemDTO = new TodoItemDTO(todoItem);

        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
    }
    else
    {
        todo.Id = todoItemDTO.Id;
        todo.lat = todoItemDTO.lat;
        todo.lon = todoItemDTO.lon;
        todo.aqi = todoItemDTO.aqi;


        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }

}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}