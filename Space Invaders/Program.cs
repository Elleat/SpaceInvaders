using System.Collections.Generic;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Space_Invaders.Pages;

var builder = WebApplication.CreateBuilder(args);

// ��������� �������
builder.Services.AddRazorPages();

// ��� API �����������
builder.Services.AddControllers();

builder.Services.AddDbContext<SpaceInvaderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(SpaceInvaderDbContext).Assembly.FullName)));

var app = builder.Build();

// ������������ pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ��������
app.MapRazorPages();
app.MapControllers(); // ��� API �����������



// ������������� ����-������ ���� �����
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
            PlayerName = $"����� {i + 1}",
            Score = baseScore - (i * 100),
            Date = DateTime.Now.AddDays(-i)
        });
    }
}

// ����������� ��������� �����������
public static class GameData
{
    public static List<ScoresModel.PlayerScore> AppScores { get; set; } = new List<ScoresModel.PlayerScore>();
}