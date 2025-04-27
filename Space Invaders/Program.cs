using System.Collections.Generic;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Space_Invaders.Pages;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddRazorPages();

// Для API контроллера
builder.Services.AddControllers();

builder.Services.AddDbContext<SpaceInvaderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(SpaceInvaderDbContext).Assembly.FullName)));

var app = builder.Build();

// Конфигурация pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Маршруты
app.MapRazorPages();
app.MapControllers(); // Для API контроллера



// Инициализация демо-данных если пусто
if (GameData.AppScores.Count == 0)
{
    InitializeDemoScores();
}

app.Run();

void InitializeDemoScores()
{
    int baseScore = 1000;
    for (int i = 0; i < 10; i++)
    {
        GameData.AppScores.Add(new ScoresModel.PlayerScore
        {
            PlayerName = $"Игрок {i + 1}",
            Score = baseScore - (i * 100),
            Date = DateTime.Now.AddDays(-i)
        });
    }
}

// Статическое хранилище результатов
public static class GameData
{
    public static List<ScoresModel.PlayerScore> AppScores { get; set; } = new List<ScoresModel.PlayerScore>();
}