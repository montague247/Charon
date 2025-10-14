using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("latest");
builder.Services.AddCors(o => o.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// JWT Authentication hinzufügen
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        var validIssuer = jwtSettings["ValidIssuer"];
        var validAudience = jwtSettings["ValidAudience"];

        // use [Authorize] to protect endpoints
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(validIssuer),
            ValidateAudience = !string.IsNullOrEmpty(validAudience),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = validIssuer,
            ValidAudience = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? "YOUR_SECRET_KEY"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication(); // JWT Authentifizierung aktivieren
app.UseAuthorization();

app.MapControllers();

app.Run();
