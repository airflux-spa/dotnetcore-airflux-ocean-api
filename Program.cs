using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;
using System.Text;
using Microsoft.Azure.Devices.Client;
//using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Text.Json; // Asegúrate de tener esta referencia


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<JsonOptions>(options =>
{
    // Set this to true to ignore null or default values
    options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddHttpClient();
var app = builder.Build();

app.UseHttpsRedirection(); // Redirige HTTP a HTTPS, si es necesario
app.UseRouting(); // Habilita el enrutamiento de solicitudes

app.UseCors("AllowAll");

//our custom middleware extension to call UseMiddleware
app.UseMiddleware<ApiKeyMiddleware>();

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");
RouteGroupBuilder todoItems2 = app.MapGroup("/todoitems2");
RouteGroupBuilder interior = app.MapGroup("/interior");
RouteGroupBuilder exterior = app.MapGroup("/exterior");

todoItems.MapGet("/", GetExterior);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

todoItems2.MapGet("/", GetAllTodoswtime);
interior.MapGet("/", GetInterior);
exterior.MapGet("/", GetExterior);

RouteGroupBuilder thingspeak = app.MapGroup("/thingspeak");
thingspeak.MapGet("/", GetThingSpeakData);

// RouteGroupBuilder thingspeak2 = app.MapGroup("/thingspeak2");
// thingspeak2.MapGet("/", GetThingSpeakData2);

app.Run();

static async Task<IResult> GetExterior(TodoDb db)
{
    var cutoffTime = DateTime.Now.AddMinutes(-20); // Calcula el tiempo límite hace 20 minutos

    var idsEntradasMasAntiguas = await db.Todos
        .Where(entry => entry.Datet <= cutoffTime)
        .Select(entry => entry.Id) // Proyecta solo el ID
        .ToListAsync();

    // borra las entradas antiguas
    foreach (var id in idsEntradasMasAntiguas)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
        }
    }

    // Solo lista nodos exteriores
    return TypedResults.Ok(await db.Todos
    .Where(x => x.Env != null && x.Env == 0) // Filtrar por Id > 1
    .Select(x => new TodoItemDTO
    {
        Id = x.Id,
        Lat = x.Lat,
        Lon = x.Lon,
        Aqi = x.Aqi,
    }).ToArrayAsync());
}


static async Task<IResult> GetAllTodoswtime(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
}


static async Task<IResult> GetInterior(TodoDb db)
{
    var cutoffTime = DateTime.Now.AddMinutes(-20); // Calcula el tiempo límite hace 20 minutos

    var idsEntradasMasAntiguas = await db.Todos
        .Where(entry => entry.Datet <= cutoffTime)
        .Select(entry => entry.Id) // Proyecta solo el ID
        .ToListAsync();

    // borra las entradas antiguas
    foreach (var id in idsEntradasMasAntiguas)
    {
        if (await db.Todos.FindAsync(id) is Todo todo)
        {
            db.Todos.Remove(todo);
            await db.SaveChangesAsync();
        }
    }

    // Solo lista nodos exteriores
    return TypedResults.Ok(await db.Todos
    .Where(x => x.Env != null && x.Env == 1) // Filtrar por Id > 1
    .Select(x => new TodoItemDTO
    {
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


static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db, HttpContext context)
{
    var todo = await db.Todos.FindAsync(todoItemDTO.Id);

    if (todo is null)
    {
        var todoItem = new Todo
        {
            Id = todoItemDTO.Id,
            Deviceid = todoItemDTO.Deviceid,
            Tskey = todoItemDTO.Tskey,
            Tschannel = todoItemDTO.Tschannel,
            Apikey = todoItemDTO.Apikey,
            Lat = todoItemDTO.Lat,
            Lon = todoItemDTO.Lon,
            Env = todoItemDTO.Env,
            Priv = todoItemDTO.Priv,
            Tem = todoItemDTO.Tem,
            Pre = todoItemDTO.Pre,
            Hum = todoItemDTO.Hum,
            Pm10 = todoItemDTO.Pm10,
            Pm25 = todoItemDTO.Pm25,
            Riesgo = todoItemDTO.Riesgo,
            //Aqi = todoItemDTO.Aqi,
            Datet = todoItemDTO.Datet,
        };

        db.Todos.Add(todoItem);
        await db.SaveChangesAsync();

        todoItemDTO = new TodoItemDTO(todoItem);

        return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
    }
    else
    {
        todo.Id = todoItemDTO.Id;
        todo.Deviceid = todoItemDTO.Deviceid;
        todo.Tskey = todoItemDTO.Tskey;
        todo.Tschannel = todoItemDTO.Tschannel;
        todo.Apikey = todoItemDTO.Apikey;
        todo.Lat = todoItemDTO.Lat;
        todo.Lon = todoItemDTO.Lon;
        todo.Env = todoItemDTO.Env;
        todo.Priv = todoItemDTO.Priv;
        todo.Tem = todoItemDTO.Tem;
        todo.Pre = todoItemDTO.Pre;
        todo.Hum = todoItemDTO.Hum;
        todo.Pm10 = todoItemDTO.Pm10;
        todo.Pm25 = todoItemDTO.Pm25;
        todo.Riesgo = todoItemDTO.Riesgo;
        //todo.Aqi = todoItemDTO.Aqi;
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

static async Task<IResult> GetThingSpeakData(TodoDb db)
{
    var oneHourAgo = DateTime.Now.AddHours(-24); // Calcula el tiempo límite de las últimas 24 horas

    var todos = await db.Todos
        .Where(x => x.Env == 0 && x.Priv == 0 && x.Datet >= oneHourAgo)
        .Select(x => new TodoItemDTO
        {
            // Id = x.Id,
            Tschannel = x.Tschannel,
            Apikey = x.Apikey,
            // Lat = x.Lat,
            // Lon = x.Lon,
            // Env = x.Env,
            // Priv = x.Priv,
            // Datet = x.Datet

        }).ToArrayAsync();

    //return TypedResults.Ok(todos);
    string jsonData = JsonSerializer.Serialize(todos);
    string encryptedData = EncryptionHelper.EncryptString(jsonData);

    // Devolver la respuesta como texto sin formato
    return Results.Text(encryptedData, "text/plain");
}

