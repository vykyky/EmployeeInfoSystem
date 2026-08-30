using EmployeeInfoSystem.API.Middleware;
using EmployeeInfoSystem.Application;
using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Domain;
using EmployeeInfoSystem.Infrastructure;
using EmployeeInfoSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    // 1. Инициализация админа (ваш текущий код)
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    var adminExists = await uow.Users.GetByTabnAsync("admin");

    if (adminExists == null)
    {
        var admin = new User
        {
            Tabn = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "admin",
            CreatedAt = DateTime.UtcNow
        };

        await uow.Users.AddAsync(admin);
        await uow.SaveChangesAsync();
    }

    // =========================================================================
    // 2. ДИНАМИЧЕСКИЙ СИД ДЛЯ ТИПОВ ЗАПРОСОВ (Добавляем сюда)
    // =========================================================================
    // Достаем контекст напрямую или через репозитории (если у вас есть uow.RequestTypes)
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Проверяем, есть ли уже записи в таблице requesttypes
    if (!dbContext.RequestTypes.Any())
    {
        var defaultTypes = new List<RequestType>
        {
            new RequestType
            {
                Name = "Изменение контактных данных",
                IsActive = true,
                IsSystem = true,
                Code = "CHANGE_CONTACTS"
            },
            new RequestType
            {
                Name = "Изменение размеров спецодежды",
                IsActive = true,
                IsSystem = true,
                Code = "CHANGE_SIZES"
            }
        };

        await dbContext.RequestTypes.AddRangeAsync(defaultTypes);
        await dbContext.SaveChangesAsync();
    }
}

app.UseCors("ReactPolicy");


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();



AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

app.MapControllers();

app.Run();
