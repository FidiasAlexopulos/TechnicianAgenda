using Microsoft.EntityFrameworkCore;
using TechnicianAgenda.Data;
using TechnicianAgenda.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen();

// Add this to fix the infinite loop
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Add database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Redis caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "TechnicianAgenda_";
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/clients", async (AppDbContext db) =>
{
    var clients = await db.Clients.Include(c => c.Directions).ToListAsync();
    return Results.Ok(clients);
});

// Get a single client by ID
app.MapGet("/api/clients/{id}", async (int id, AppDbContext db) =>
{
    var client = await db.Clients
        .Include(c => c.Directions)
        .FirstOrDefaultAsync(c => c.Id == id);

    return client is not null ? Results.Ok(client) : Results.NotFound();
});

// Create a new client
app.MapPost("/api/clients", async (Client client, AppDbContext db) =>
{
    db.Clients.Add(client);
    await db.SaveChangesAsync();
    return Results.Created($"/api/clients/{client.Id}", client);
});

// ============================================
// DIRECTION ENDPOINTS
// ============================================

// Get all directions for a specific client
app.MapGet("/api/clients/{clientId}/directions", async (int clientId, AppDbContext db) =>
{
    var directions = await db.Directions
        .Where(d => d.ClientId == clientId)
        .ToListAsync();

    return Results.Ok(directions);
});

// Create a new direction for a client
app.MapPost("/api/directions", async (Direction direction, AppDbContext db) =>
{
    // Check if client exists
    var clientExists = await db.Clients.AnyAsync(c => c.Id == direction.ClientId);
    if (!clientExists)
    {
        return Results.BadRequest("Client not found");
    }

    db.Directions.Add(direction);
    await db.SaveChangesAsync();
    return Results.Created($"/api/directions/{direction.Id}", direction);
});

// ============================================
// WORK ORDER ENDPOINTS
// ============================================

// Get all work orders
// Get all work orders (with caching)
// Get all work orders (with caching)
app.MapGet("/api/works", async (AppDbContext db, IDistributedCache cache, ILogger<Program> logger) =>
{
    var cacheKey = "all_works";

    // Define JSON options once for the whole method
    var jsonOptions = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    };

    // Try to get from cache first
    var cachedData = await cache.GetStringAsync(cacheKey);

    if (!string.IsNullOrEmpty(cachedData))
    {
        logger.LogInformation("✅ Returning data from CACHE!");
        var cachedWorks = JsonSerializer.Deserialize<List<Work>>(cachedData, jsonOptions);
        return Results.Ok(cachedWorks);
    }

    // If not in cache, get from database
    logger.LogInformation("📊 Fetching from DATABASE...");
    var works = await db.Works
        .Include(w => w.Client)
        .Include(w => w.Direction)
        .ToListAsync();

    // Store in cache for 5 minutes
    var serializedData = JsonSerializer.Serialize(works, jsonOptions);
    var cacheOptions = new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };
    await cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

    return Results.Ok(works);
});

// Get a single work order by ID
app.MapGet("/api/works/{id}", async (int id, AppDbContext db) =>
{
    var work = await db.Works
        .Include(w => w.Client)
        .Include(w => w.Direction)
        .FirstOrDefaultAsync(w => w.Id == id);

    return work is not null ? Results.Ok(work) : Results.NotFound();
});

// Create a new work order
// Create a new work order (and clear cache)
app.MapPost("/api/works", async (Work work, AppDbContext db, IDistributedCache cache) =>
{
    var clientExists = await db.Clients.AnyAsync(c => c.Id == work.ClientId);
    if (!clientExists)
    {
        return Results.BadRequest("Client not found");
    }

    var direction = await db.Directions
        .FirstOrDefaultAsync(d => d.Id == work.DirectionId && d.ClientId == work.ClientId);

    if (direction is null)
    {
        return Results.BadRequest("Direction not found or doesn't belong to the specified client");
    }

    db.Works.Add(work);
    await db.SaveChangesAsync();

    // Clear cache when new work is created
    await cache.RemoveAsync("all_works");
    Console.WriteLine("🗑️ Cache cleared!");

    return Results.Created($"/api/works/{work.Id}", work);
});

// Update work status (mark as done/not done)
app.MapPatch("/api/works/{id}/status", async (int id, bool status, AppDbContext db) =>
{
    var work = await db.Works.FindAsync(id);
    if (work is null)
    {
        return Results.NotFound();
    }

    work.Status = status;
    await db.SaveChangesAsync();
    return Results.Ok(work);
});

app.Run();