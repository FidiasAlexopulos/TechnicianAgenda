using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TechnicianAgenda.Data;
using TechnicianAgenda.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Fix for circular references in JSON
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Add database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();



// Add caching - Usa Redis si está disponible, sino usa memoria
var redisConnection = builder.Configuration.GetValue<string>("Redis:ConnectionString");
if (!string.IsNullOrEmpty(redisConnection) && redisConnection != "localhost:6379")
{
    try
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "TechnicianAgenda_";
        });
        Console.WriteLine("✅ Using Redis cache");
    }
    catch
    {
        builder.Services.AddDistributedMemoryCache();
        Console.WriteLine("⚠️ Redis failed, using memory cache");
    }
}
else
{
    builder.Services.AddDistributedMemoryCache();
    Console.WriteLine("ℹ️ Using memory cache (no Redis configured)");
}

// Add CORS for React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
                policy =>
            {
            policy.WithOrigins(
                "http://localhost:3000",
                "https://technician-agenda.vercel.app",
                "https://technician-agenda-git-main-fidias-projects-347e50da.vercel.app"
            )
                            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


var app = builder.Build();



// Configure the HTTP request pipeline

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

// Create uploads directory if it doesn't exist
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}
app.UseStaticFiles(); // Serve static files from wwwroot

// ============================================
// AUTHENTICATION ENDPOINTS
// ============================================

app.MapPost("/api/auth/register", async (RegisterRequest request, AppDbContext db) =>
{
    // Validar que el username no exista
    if (await db.Users.AnyAsync(u => u.Username == request.Username))
    {
        return Results.BadRequest(new { message = "El nombre de usuario ya existe" });
    }

    // Validar que el email no exista
    if (await db.Users.AnyAsync(u => u.Email == request.Email))
    {
        return Results.BadRequest(new { message = "El email ya está registrado" });
    }

    // Crear usuario
    var user = new User
    {
        Username = request.Username,
        Email = request.Email,
        FullName = request.FullName,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    // Generar token
    var token = GenerateJwtToken(user);

    return Results.Ok(new AuthResponse
    {
        Token = token,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName
    });
});

app.MapPost("/api/auth/login", async (LoginRequest request, AppDbContext db) =>
{
    // Buscar usuario
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    if (!user.IsActive)
    {
        return Results.BadRequest(new { message = "Usuario inactivo" });
    }

    // Generar token
    var token = GenerateJwtToken(user);

    return Results.Ok(new AuthResponse
    {
        Token = token,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName
    });
});

// Función helper para generar JWT
string GenerateJwtToken(User user)
{
    var jwtKey = builder.Configuration["Jwt:Key"]!;
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];
    var expirationHours = double.Parse(builder.Configuration["Jwt:ExpirationInHours"] ?? "24");

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("FullName", user.FullName)
    };

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(expirationHours),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

// ============================================
// CLIENT ENDPOINTS
// ============================================

app.MapGet("/api/clients", [Authorize] async (AppDbContext db, HttpContext context) =>

{
    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    var clients = await db.Clients
     .Where(c => c.UserId == userId && c.IsActive)    // ← FILTRO CLAVE
     .Include(c => c.Directions)
     .ToListAsync();

    return Results.Ok(clients);
});
app.MapGet("/api/clients/{id}", [Authorize] async (int id, AppDbContext db, HttpContext context) =>
{
    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // Buscar el cliente SOLO si pertenece al usuario autenticado
    var client = await db.Clients
        .Where(c => c.UserId == userId && c.IsActive)  // ← Verificar propiedad
        .Include(c => c.Directions)
        .FirstOrDefaultAsync(c => c.Id == id);

    return client is not null ? Results.Ok(client) : Results.NotFound();
});
app.MapPost("/api/clients", [Authorize] async (Client client, AppDbContext db, HttpContext context) =>
{
    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // Asignar el cliente al usuario autenticado
    client.UserId = userId;  // ← ASIGNACIÓN CLAVE

    db.Clients.Add(client);
    await db.SaveChangesAsync();

    return Results.Created($"/api/clients/{client.Id}", client);
});
app.MapPut("/api/clients/{id}", [Authorize] async (int id, Client updatedClient, AppDbContext db, HttpContext context) =>
{
    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // Buscar el cliente SOLO si pertenece al usuario
    var client = await db.Clients
        .Where(c => c.UserId == userId)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (client is null)
    {
        return Results.NotFound();
    }

    // Actualizar campos
    client.Name = updatedClient.Name;
    client.Apellidos = updatedClient.Apellidos;
    client.Telefono = updatedClient.Telefono;
    client.CorreoElectronico = updatedClient.CorreoElectronico;
    // NO actualizar UserId - debe permanecer igual

    await db.SaveChangesAsync();

    return Results.Ok(client);
});
app.MapDelete("/api/clients/{id}", [Authorize] async (int id, AppDbContext db, HttpContext context) =>
{
    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // Buscar el cliente SOLO si pertenece al usuario
    var client = await db.Clients
        .Where(c => c.UserId == userId)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (client is null)
    {
        return Results.NotFound();
    }

    client.IsActive = false;
    await db.SaveChangesAsync();

    return Results.NoContent();  // 204 No Content = eliminado exitosamente
});
app.MapPatch("/api/clients/{id}/restore", [Authorize] async (int id, AppDbContext db, HttpContext context) =>
{
    var userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    var client = await db.Clients
        .Where(c => c.UserId == userId && !c.IsActive)  // Solo inactivos
        .FirstOrDefaultAsync(c => c.Id == id);

    if (client is null)
    {
        return Results.NotFound();
    }

    client.IsActive = true;  // Restaurar
    await db.SaveChangesAsync();

    return Results.Ok(client);
});

// ============================================
// DIRECTION ENDPOINTS
// ============================================

app.MapGet("/api/clients/{clientId}/directions", [Authorize] async (int clientId, AppDbContext db) =>
{
    var directions = await db.Directions
        .Where(d => d.ClientId == clientId)
        .ToListAsync();

    return Results.Ok(directions);
});

app.MapPost("/api/directions", [Authorize] async (Direction direction, AppDbContext db) =>
{
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
// REGIONS AND COMUNAS ENDPOINTS
// ============================================

app.MapGet("/api/regions", () =>
{
    var regions = RegionHelper.RegionNames.Select(r => new
    {
        id = (int)r.Key,
        name = r.Value
    }).ToList();

    return Results.Ok(regions);
});

app.MapGet("/api/regions/{regionId}/comunas", (int regionId) =>
{
    var region = (Region)regionId;

    if (!RegionHelper.ComunasByRegion.ContainsKey(region))
    {
        return Results.NotFound("Region not found");
    }

    var comunas = RegionHelper.ComunasByRegion[region];
    return Results.Ok(comunas);
});

// ============================================
// TECHNICIAN ENDPOINTS
// ============================================

app.MapGet("/api/technicians", [Authorize] async (AppDbContext db) =>
{
    var technicians = await db.Technicians
        .OrderBy(t => t.Apellidos)
        .ThenBy(t => t.Nombre)
        .ToListAsync();

    return Results.Ok(technicians);
});

app.MapGet("/api/technicians/{id}", [Authorize] async (int id, AppDbContext db) =>
{
    var technician = await db.Technicians
        .Include(t => t.AssignedWorks)
        .FirstOrDefaultAsync(t => t.Id == id);

    return technician is not null ? Results.Ok(technician) : Results.NotFound();
});

app.MapPost("/api/technicians", [Authorize] async (HttpRequest request, AppDbContext db) =>
{
    try
    {
        var form = await request.ReadFormAsync();

        var technician = new Technician
        {
            Nombre = form["nombre"].ToString(),
            Apellidos = form["apellidos"].ToString(),
            Nacionalidad = form["nacionalidad"].ToString(),
            RutOPasaporte = form["rutOPasaporte"].ToString(),
            FechaNacimiento = DateTime.Parse(form["fechaNacimiento"].ToString()),
            Region = (Region)int.Parse(form["region"].ToString()),
            Comuna = form["comuna"].ToString(),
            Direccion = form["direccion"].ToString(),
            CorreoElectronico = form["correoElectronico"].ToString(),
            NumeroTelefonico = form["numeroTelefonico"].ToString(),
            NumeroTelefonicoAlternativo = form["numeroTelefonicoAlternativo"].ToString(),
            PatenteVehiculo = form["patenteVehiculo"].ToString(),
            Certificaciones = form["certificaciones"].ToString()
        };

        // Handle photo upload
        var photo = form.Files.GetFile("photo");
        if (photo != null && photo.Length > 0)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "technicians");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            technician.FotografiaPath = $"/uploads/technicians/{fileName}";
        }

        // Check if RUT/Passport already exists
        var exists = await db.Technicians.AnyAsync(t => t.RutOPasaporte == technician.RutOPasaporte);
        if (exists)
        {
            return Results.BadRequest("Ya existe un técnico con ese RUT o Pasaporte");
        }

        db.Technicians.Add(technician);
        await db.SaveChangesAsync();

        return Results.Created($"/api/technicians/{technician.Id}", technician);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/technicians/{id}", [Authorize] async (int id, HttpRequest request, AppDbContext db) =>
{
    try
    {
        var technician = await db.Technicians.FindAsync(id);
        if (technician is null)
        {
            return Results.NotFound();
        }

        var form = await request.ReadFormAsync();

        technician.Nombre = form["nombre"].ToString();
        technician.Apellidos = form["apellidos"].ToString();
        technician.Nacionalidad = form["nacionalidad"].ToString();
        technician.RutOPasaporte = form["rutOPasaporte"].ToString();
        technician.FechaNacimiento = DateTime.Parse(form["fechaNacimiento"].ToString());
        technician.Region = (Region)int.Parse(form["region"].ToString());
        technician.Comuna = form["comuna"].ToString();
        technician.Direccion = form["direccion"].ToString();
        technician.CorreoElectronico = form["correoElectronico"].ToString();
        technician.NumeroTelefonico = form["numeroTelefonico"].ToString();
        technician.NumeroTelefonicoAlternativo = form["numeroTelefonicoAlternativo"].ToString();
        technician.PatenteVehiculo = form["patenteVehiculo"].ToString();
        technician.Certificaciones = form["certificaciones"].ToString();

        // Handle photo upload if new photo provided
        var photo = form.Files.GetFile("photo");
        if (photo != null && photo.Length > 0)
        {
            // Delete old photo if exists
            if (!string.IsNullOrEmpty(technician.FotografiaPath))
            {
                var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", technician.FotografiaPath.TrimStart('/'));
                if (File.Exists(oldPhotoPath))
                {
                    File.Delete(oldPhotoPath);
                }
            }

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "technicians");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            technician.FotografiaPath = $"/uploads/technicians/{fileName}";
        }

        await db.SaveChangesAsync();
        return Results.Ok(technician);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/api/technicians/{id}", [Authorize] async (int id, AppDbContext db) =>
{
    var technician = await db.Technicians.FindAsync(id);
    if (technician is null)
    {
        return Results.NotFound();
    }

    // Delete photo if exists
    if (!string.IsNullOrEmpty(technician.FotografiaPath))
    {
        var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", technician.FotografiaPath.TrimStart('/'));
        if (File.Exists(photoPath))
        {
            File.Delete(photoPath);
        }
    }

    db.Technicians.Remove(technician);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Get technicians summary for work assignment dropdown
app.MapGet("/api/technicians/summary", [Authorize] async (AppDbContext db) =>
{
    var technicians = await db.Technicians
        .Select(t => new
        {
            t.Id,
            t.Nombre,
            t.Apellidos,
            t.RutOPasaporte,
            NombreCompleto = $"{t.Nombre} {t.Apellidos}"
        })
        .OrderBy(t => t.Apellidos)
        .ToListAsync();

    return Results.Ok(technicians);
});


// ============================================
// JOB CATEGORIES ENDPOINTS
// ============================================

app.MapGet("/api/jobcategories", async (AppDbContext db) =>
{
    var categories = await db.JobCategories
        .Include(c => c.Subcategories)
        .OrderBy(c => c.DisplayOrder)
        .ToListAsync();

    return Results.Ok(categories);
});

app.MapGet("/api/jobcategories/{categoryId}/subcategories", async (int categoryId, AppDbContext db) =>
{
    var subcategories = await db.JobSubcategories
        .Where(s => s.JobCategoryId == categoryId)
        .OrderBy(s => s.DisplayOrder)
        .ToListAsync();

    return Results.Ok(subcategories);
});

// ============================================
// WORK ENDPOINTS (ACTUALIZADO CON TECHNICIAN)
// ============================================

// REEMPLAZA tu endpoint GET /api/works actual con este:

app.MapGet("/api/works", [Authorize] async (AppDbContext db, IDistributedCache cache) =>
{
    var cacheKey = "all_works";
    var cachedWorks = await cache.GetStringAsync(cacheKey);

    if (!string.IsNullOrEmpty(cachedWorks))
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
        var works = JsonSerializer.Deserialize<List<Work>>(cachedWorks, options);
        return Results.Ok(works);
    }

    var worksFromDb = await db.Works
        .Include(w => w.Client)
        .Include(w => w.Direction)
        .Include(w => w.JobCategory)
        .Include(w => w.JobSubcategory)
        .Include(w => w.Files)
        .Include(w => w.Technician) // Incluir técnico
        .AsNoTracking() // IMPORTANTE: Evita tracking para prevenir ciclos
        .OrderByDescending(w => w.Date)
        .ToListAsync();

    var options2 = new JsonSerializerOptions
    {
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    var serializedWorks = JsonSerializer.Serialize(worksFromDb, options2);
    await cache.SetStringAsync(cacheKey, serializedWorks, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });

    return Results.Ok(worksFromDb);
});

app.MapGet("/api/works/{id}", [Authorize] async (int id, AppDbContext db) =>
{
    var work = await db.Works
        .Include(w => w.Client)
        .Include(w => w.Direction)
        .Include(w => w.JobCategory)
        .Include(w => w.JobSubcategory)
        .Include(w => w.Files)
        .Include(w => w.Technician) // NUEVO: Incluir técnico
        .FirstOrDefaultAsync(w => w.Id == id);

    return work is not null ? Results.Ok(work) : Results.NotFound();
});

app.MapPost("/api/works", [Authorize] async (Work work, AppDbContext db, IDistributedCache cache) =>
{
    try
    {
        // Validaciones
        var clientExists = await db.Clients.AnyAsync(c => c.Id == work.ClientId);
        if (!clientExists)
        {
            return Results.BadRequest("Client not found");
        }

        var directionExists = await db.Directions.AnyAsync(d => d.Id == work.DirectionId);
        if (!directionExists)
        {
            return Results.BadRequest("Direction not found");
        }

        var categoryExists = await db.JobCategories.AnyAsync(jc => jc.Id == work.JobCategoryId);
        if (!categoryExists)
        {
            return Results.BadRequest("Job category not found");
        }

        var subcategoryExists = await db.JobSubcategories.AnyAsync(js => js.Id == work.JobSubcategoryId);
        if (!subcategoryExists)
        {
            return Results.BadRequest("Job subcategory not found");
        }

        // NUEVO: Validar técnico si se proporciona
        if (work.TechnicianId.HasValue)
        {
            var technicianExists = await db.Technicians.AnyAsync(t => t.Id == work.TechnicianId.Value);
            if (!technicianExists)
            {
                return Results.BadRequest("Technician not found");
            }
        }

        db.Works.Add(work);
        await db.SaveChangesAsync();

        // Invalidar caché
        await cache.RemoveAsync("all_works");

        // Cargar relaciones para la respuesta
        await db.Entry(work).Reference(w => w.Client).LoadAsync();
        await db.Entry(work).Reference(w => w.Direction).LoadAsync();
        await db.Entry(work).Reference(w => w.JobCategory).LoadAsync();
        await db.Entry(work).Reference(w => w.JobSubcategory).LoadAsync();
        if (work.TechnicianId.HasValue)
        {
            await db.Entry(work).Reference(w => w.Technician).LoadAsync();
        }

        return Results.Created($"/api/works/{work.Id}", work);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/works/{id}", [Authorize] async (int id, Work updatedWork, AppDbContext db, IDistributedCache cache) =>
{
    var work = await db.Works.FindAsync(id);
    if (work is null)
    {
        return Results.NotFound();
    }

    // Validaciones
    if (updatedWork.ClientId != work.ClientId)
    {
        var clientExists = await db.Clients.AnyAsync(c => c.Id == updatedWork.ClientId);
        if (!clientExists)
        {
            return Results.BadRequest("Client not found");
        }
    }

    if (updatedWork.DirectionId != work.DirectionId)
    {
        var directionExists = await db.Directions.AnyAsync(d => d.Id == updatedWork.DirectionId);
        if (!directionExists)
        {
            return Results.BadRequest("Direction not found");
        }
    }

    // NUEVO: Validar técnico si se proporciona
    if (updatedWork.TechnicianId.HasValue)
    {
        var technicianExists = await db.Technicians.AnyAsync(t => t.Id == updatedWork.TechnicianId.Value);
        if (!technicianExists)
        {
            return Results.BadRequest("Technician not found");
        }
    }

    // Actualizar propiedades
    work.JobCategoryId = updatedWork.JobCategoryId;
    work.JobSubcategoryId = updatedWork.JobSubcategoryId;
    work.Detalles = updatedWork.Detalles;
    work.Date = updatedWork.Date;
    work.Status = updatedWork.Status;
    work.ClientId = updatedWork.ClientId;
    work.DirectionId = updatedWork.DirectionId;
    work.Costos = updatedWork.Costos;
    work.TotalACobrar = updatedWork.TotalACobrar;
    work.PaymentStatus = updatedWork.PaymentStatus;

    // NUEVO: Actualizar campos de técnico
    work.TechnicianId = updatedWork.TechnicianId;
    work.PorPagarATecnico = updatedWork.PorPagarATecnico;
    work.PagoATecnicoRealizado = updatedWork.PagoATecnicoRealizado;

    await db.SaveChangesAsync();
    await cache.RemoveAsync("all_works");

    return Results.Ok(work);
});

app.MapPatch("/api/works/{id}/status", [Authorize] async (int id, bool status, AppDbContext db, IDistributedCache cache) =>
{
    var work = await db.Works.FindAsync(id);
    if (work is null)
    {
        return Results.NotFound();
    }

    work.Status = status;
    await db.SaveChangesAsync();
    await cache.RemoveAsync("all_works");

    return Results.Ok(work);
});

app.MapPatch("/api/works/{id}/technician-payment", [Authorize] async (int id, bool paid, AppDbContext db, IDistributedCache cache) =>
{
    var work = await db.Works.FindAsync(id);
    if (work is null)
    {
        return Results.NotFound();
    }

    work.PagoATecnicoRealizado = paid;
    await db.SaveChangesAsync();
    await cache.RemoveAsync("all_works");

    return Results.Ok(work);
});

app.MapDelete("/api/works/{id}", [Authorize] async (int id, AppDbContext db, IDistributedCache cache) =>
{
    var work = await db.Works.Include(w => w.Files).FirstOrDefaultAsync(w => w.Id == id);
    if (work is null)
    {
        return Results.NotFound();
    }

    // Delete associated files from disk
    foreach (var file in work.Files)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    db.Works.Remove(work);
    await db.SaveChangesAsync();
    await cache.RemoveAsync("all_works");

    return Results.NoContent();
});

// ============================================
// FILE UPLOAD ENDPOINTS
// ============================================

app.MapPost("/api/works/{workId}/files", [Authorize] async (int workId, HttpRequest request, AppDbContext db, IDistributedCache cache) =>
{
    var work = await db.Works.FindAsync(workId);
    if (work is null)
    {
        return Results.NotFound("Work not found");
    }

    if (!request.HasFormContentType)
    {
        return Results.BadRequest("No files uploaded");
    }

    var form = await request.ReadFormAsync();
    var files = form.Files;

    if (files.Count == 0)
    {
        return Results.BadRequest("No files uploaded");
    }

    var uploadedFiles = new List<WorkFile>();
    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    foreach (var file in files)
    {
        if (file.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileType = file.ContentType.StartsWith("image/") ? "image" : "video";

            var workFile = new WorkFile
            {
                FileName = file.FileName,
                FilePath = $"/uploads/{fileName}",
                FileType = fileType,
                WorkId = workId
            };

            db.WorkFiles.Add(workFile);
            uploadedFiles.Add(workFile);
        }
    }

    await db.SaveChangesAsync();
    await cache.RemoveAsync("all_works");

    return Results.Ok(uploadedFiles);
});

app.MapGet("/api/works/{workId}/files", [Authorize] async (int workId, AppDbContext db) =>
{
    var files = await db.WorkFiles
        .Where(f => f.WorkId == workId)
        .ToListAsync();

    return Results.Ok(files);
});

app.MapDelete("/api/files/{fileId}", [Authorize] async (int fileId, AppDbContext db, IDistributedCache cache) =>
{
    var file = await db.WorkFiles.FindAsync(fileId);
    if (file is null)
    {
        return Results.NotFound();
    }

    // Delete physical file
    var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('/'));
    if (File.Exists(physicalPath))
    {
        File.Delete(physicalPath);
    }

    db.WorkFiles.Remove(file);
    await db.SaveChangesAsync();
    await cache.RemoveAsync("all_works");

    return Results.Ok();
});

// ============================================
// SEED DATA
// ============================================


    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!await db.JobCategories.AnyAsync())
    {
        var categories = new List<JobCategory>
        {
            new JobCategory { Name = "Calefont y Gas", Icon = "🔥", DisplayOrder = 1,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Cambio de regulador y mangueras", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Certificación de instalaciones de gas", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Conversión gas natural / gas licuado", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Detección de fugas de gas", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Instalación de calefont", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Mantención preventiva", DisplayOrder = 6 },
                    new JobSubcategory { Name = "Reparación de calefont", DisplayOrder = 7 },
                    new JobSubcategory { Name = "Reparación de fugas de gas", DisplayOrder = 8 }
                }
            },
            new JobCategory { Name = "Cerrajería y Seguridad", Icon = "🔐", DisplayOrder = 2,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Apertura de puertas", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Cambio de cerraduras", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Copia y reparación de llaves", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Instalación de chapas de seguridad", DisplayOrder = 4 },
                       new JobSubcategory { Name = "Instalación de cerraduras digitales", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Refuerzo de puertas y ventanas", DisplayOrder = 6 }
                }
            },
            new JobCategory { Name = "Climatización", Icon = "❄️", DisplayOrder = 3,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Instalación de aire acondicionado", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Instalación de estufas eléctricas", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Limpieza de filtros", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Mantención de aire acondicionado", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Reparación de equipos", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Construcción – Obras Menores", Icon = "🧱", DisplayOrder = 4,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Instalación de cerámicas y revestimientos", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Obras menores en construcción", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Reparaciones estructurales menores", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Reparaciones post obra", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Tabiques y divisiones", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Destapes y Alcantarillado", Icon = "🪠", DisplayOrder = 5,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Destape de duchas y desagües", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Destape de lavaplatos", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Destape de WC", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Detección de fugas en alcantarillado", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Hidrolavado de cañerías", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Inspección con cámara", DisplayOrder = 6 },
                    new JobSubcategory { Name = "Limpieza de cámaras de alcantarillado", DisplayOrder = 7 }
                }
            },
            new JobCategory { Name = "Electricidad", Icon = "⚡", DisplayOrder = 6,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Canalización y cableado", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Cambio de automático y diferencial", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Certificación eléctrica (TE1)", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Instalación de tableros eléctricos", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Instalación y cambio de enchufes", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Instalación de luminarias y focos", DisplayOrder = 6 },
                    new JobSubcategory { Name = "Reparación de cortocircuitos", DisplayOrder = 7 }
                }
            },
            new JobCategory { Name = "Energía Solar", Icon = "☀️", DisplayOrder = 7,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Instalación de paneles solares", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Limpieza de paneles solares", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Mantención de paneles solares", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Revisión de inversores y conexiones", DisplayOrder = 4 }
                }
            },
            new JobCategory { Name = "Gasfitería y Agua", Icon = "🚰", DisplayOrder = 8,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Cambio de llaves, grifería y flexibles", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Detección de fugas de agua", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Instalación de filtros de agua", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Instalación de lavamanos y lavaplatos", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Reparación de cañerías (PVC, PPR, cobre)", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Reparación de fugas de agua", DisplayOrder = 6 },
                    new JobSubcategory { Name = "Reparación de WC, estanques y sifones", DisplayOrder = 7 }
                }
            },
            new JobCategory { Name = "Jardinería y Exteriores", Icon = "🌳", DisplayOrder = 9,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Corte de pasto", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Diseño de jardines", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Instalación de riego automático", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Mantención de jardines", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Poda de árboles y arbustos", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Limpieza y Mantención de Estanques de Agua", Icon = "🚰", DisplayOrder = 10,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Informe de análisis microbiológico certificado (opcional)", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Inspección y mantención preventiva", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Lavado y vaciado del estanque", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Limpieza de estanques", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Limpieza manual del estanque", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Remoción de residuos", DisplayOrder = 6 },
                    new JobSubcategory { Name = "Sanitización", DisplayOrder = 7 }
                }
            },
            new JobCategory { Name = "Mantención de Fosas Sépticas", Icon = "🚽", DisplayOrder = 11,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Inspección y mantención preventiva", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Lavado y sanitización de fosa", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Limpieza de fosas sépticas", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Retiro y disposición de residuos", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Vaciado de fosa", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Mueblería y Carpintería", Icon = "🪑", DisplayOrder = 12,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Ajuste de puertas y cajones", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Armado de muebles", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Cambio de bisagras y correderas", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Fabricación de muebles a medida", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Instalación de repisas y closets", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Reparación de muebles", DisplayOrder = 6 }
                }
            },
            new JobCategory { Name = "Pintura y Terminaciones", Icon = "🎨", DisplayOrder = 13,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Barnizado y lacado de madera", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Instalación de papel mural", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Pintura interior y/o exterior", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Reparación de muros y grietas", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Terminaciones decorativas", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Piscina y Hot Tub", Icon = "🏊", DisplayOrder = 14,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Aspirado y retiro de residuos", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Control y ajuste químico del agua", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Detección y reparación de fugas", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Instalación y mantención de calentadores de agua", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Limpieza de fondo y paredes", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Limpieza y cambio de filtros", DisplayOrder = 6 },
                    new JobSubcategory { Name = "Limpieza y mantención de hot tub / jacuzzi", DisplayOrder = 7 },
                    new JobSubcategory { Name = "Limpieza y mantención de piscinas", DisplayOrder = 8 },
                    new JobSubcategory { Name = "Puesta en marcha y cierre de temporada", DisplayOrder = 9 },
                    new JobSubcategory { Name = "Reparación de bombas y sistemas de filtrado", DisplayOrder = 10 }
                }
            },
            new JobCategory { Name = "Reparación de Electrodomésticos", Icon = "🔧", DisplayOrder = 15,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Campanas extractoras", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Cocinas y hornos", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Lavavajillas", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Lavadoras y secadoras", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Refrigeradores y congeladores", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Sistemas de Protección Contra Incendios", Icon = "🚒", DisplayOrder = 16,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Mantención de extintores", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Mantención de sistemas contra incendios", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Pruebas y certificación de sistemas", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Reposición y recarga de extintores", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Revisión de redes húmedas y secas", DisplayOrder = 5 }
                }
            },
            new JobCategory { Name = "Ventanas, Termopanel y Vidrios", Icon = "🪟", DisplayOrder = 17,
                Subcategories = new List<JobSubcategory> {
                    new JobSubcategory { Name = "Ajuste de cierres y correderas", DisplayOrder = 1 },
                    new JobSubcategory { Name = "Cambio de vidrios simples y dobles", DisplayOrder = 2 },
                    new JobSubcategory { Name = "Instalación de mallas de seguridad", DisplayOrder = 3 },
                    new JobSubcategory { Name = "Instalación de ventanas termopanel", DisplayOrder = 4 },
                    new JobSubcategory { Name = "Reparación de termopanel", DisplayOrder = 5 },
                    new JobSubcategory { Name = "Sellos y aislación térmica/acústica", DisplayOrder = 6 }
                }
            }
        };

        db.JobCategories.AddRange(categories);
        await db.SaveChangesAsync();

        Console.WriteLine("✅ Job categories seeded successfully!");
    }


app.Run();