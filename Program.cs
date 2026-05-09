using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Practice4.Data;
using Practice4.Models;
using Practice4.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FlowerShopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFlowerDataService, FlowerDataService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FlowerShopDbContext>();
    db.Database.EnsureCreated();

    if (!db.Flowers.Any())
    {
        db.Flowers.AddRange(
            new Flower { Name = "Роза красная", Price = 215m, Stock = 28, Category = "Классика" },
            new Flower { Name = "Тюльпан голландский", Price = 200m, Stock = 45, Category = "Сезонное" },
            new Flower { Name = "Пион", Price = 450m, Stock = 14, Category = "Премиум" },
            new Flower { Name = "Хризантема кустовая", Price = 250m, Stock = 22, Category = "Букет" },
            new Flower { Name = "Гортензия", Price = 500m, Stock = 9, Category = "Новинка" });
        db.SaveChanges();
    }
}

app.MapGet("/api/flowers", async (
    FlowerShopDbContext db,
    IFlowerDataService flowerService,
    IConfiguration configuration) =>
{
    var maxItems = configuration.GetValue<int>("AppSettings:MaxItems");
    var appVersion = configuration["AppSettings:Version"];

    var flowers = await db.Flowers.AsNoTracking().ToListAsync();
    var prepared = flowerService.PrepareForShowcase(flowers, maxItems);

    return Results.Ok(new
    {
        shopName = configuration["AppSettings:AppName"],
        appVersion,
        currency = "RUB",
        items = prepared
    });
});

app.MapGet("/api/config", (IConfiguration configuration) =>
{
    return Results.Ok(new
    {
        shopName = configuration["AppSettings:AppName"],
        version = configuration["AppSettings:Version"],
        maxItems = configuration.GetValue<int>("AppSettings:MaxItems"),
        catalogCurrency = configuration["AppSettings:Currency"],
        connectionString = configuration.GetConnectionString("DefaultConnection")
    });
});

app.MapPost("/api/flowers", async (FlowerShopDbContext db, CreateFlowerRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Category))
    {
        return Results.BadRequest(new { message = "Укажите название и категорию цветка." });
    }

    if (request.Price <= 0)
    {
        return Results.BadRequest(new { message = "Цена должна быть больше нуля." });
    }

    if (request.Stock < 0)
    {
        return Results.BadRequest(new { message = "Остаток на складе не может быть отрицательным." });
    }

    var flower = new Flower
    {
        Name = request.Name.Trim(),
        Price = request.Price,
        Stock = request.Stock,
        Category = request.Category.Trim()
    };

    db.Flowers.Add(flower);
    await db.SaveChangesAsync();

    return Results.Created($"/api/flowers/{flower.Id}", flower);
});

app.Run();

record CreateFlowerRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("stock")] int Stock,
    [property: JsonPropertyName("category")] string Category);
