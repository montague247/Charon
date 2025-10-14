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
        /*
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = "DEIN_ISSUER",
            ValidAudience = "DEINE_AUDIENCE",
            use [Authorize] to protect endpoints
        */
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "DEIN_SECRET_KEY"))
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
