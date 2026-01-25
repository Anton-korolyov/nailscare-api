using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NailsCare.Api.Auth;
using NailsCare.Api.Data;
using NailsCare.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== DATABASE =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ===== JWT =====
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
           .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.WebHost.UseUrls("http://0.0.0.0:5042");
var app = builder.Build();

// ================= PIPELINE =================

app.UseStaticFiles();
app.UseCors("frontend"); //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // ⚠️ обязательно ДО Authorization
app.UseAuthorization();

app.MapControllers();

// ================= SEED ADMIN =================
// ================= SEED ADMIN =================
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.Migrate();
 
    var phone = app.Configuration["Admin:Phone"];
    var password = app.Configuration["Admin:Password"];

    if (!db.Users.Any(x => x.Phone == phone))
    {
        db.Users.Add(new User
        {
            Phone = phone!,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password!),
            Role = "Admin"
        });

        db.SaveChanges();
    }
}
catch (Exception ex)
{
    Console.WriteLine("DB INIT ERROR:");
    Console.WriteLine(ex.Message);
}

app.Run();
