using Microsoft.EntityFrameworkCore;
using TechnicianAgenda.Data;
using TechnicianAgenda.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.MapGet("/api/works", async (AppDbContext db) =>
{
    var works = await db.Works
        .Include(w => w.Client)
        .Include(w => w.Direction)
        .ToListAsync();

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
app.MapPost("/api/works", async (Work work, AppDbContext db) =>
{
    // Validate client exists
    var clientExists = await db.Clients.AnyAsync(c => c.Id == work.ClientId);
    if (!clientExists)
    {
        return Results.BadRequest("Client not found");
    }

    // Validate direction exists and belongs to the client
    var direction = await db.Directions
        .FirstOrDefaultAsync(d => d.Id == work.DirectionId && d.ClientId == work.ClientId);

    if (direction is null)
    {
        return Results.BadRequest("Direction not found or doesn't belong to the specified client");
    }

    db.Works.Add(work);
    await db.SaveChangesAsync();
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