using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<JsonOptions>(options =>
{
    // Set this to true to ignore null or default values
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
var app = builder.Build();

//our custom middleware extension to call UseMiddleware
app.UseMiddleware<ApiKeyMiddleware>();

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

app.Run();

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO{
        Id = x.Id,
        Lat = x.Lat,
        Lon = x.Lon,
        Aqi = x.Aqi,
    }).ToArrayAsync());
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
            Lat = todoItemDTO.Lat,
            Lon = todoItemDTO.Lon,
            Aqi = todoItemDTO.Aqi,
            Datet = todoItemDTO.Datet
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        todoItemDTO = new TodoItemDTO(todoItem);

        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
    }
    else
    {
        todo.Id = todoItemDTO.Id;
        todo.Lat = todoItemDTO.Lat;
        todo.Lon = todoItemDTO.Lon;
        todo.Aqi = todoItemDTO.Aqi;
        todo.Datet = todoItemDTO.Datet;


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