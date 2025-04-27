using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Space_Invaders.Pages
{
    public class ScoresModel : PageModel
    {
        private readonly SpaceInvaderDbContext _context;
        private readonly ILogger<ScoresModel> _logger;

        public ScoresModel(SpaceInvaderDbContext context, ILogger<ScoresModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<PlayerScore> HighScores { get; set; } = new List<PlayerScore>();

        public async Task OnGetAsync()
        {
            try
            {
                // Пытаемся загрузить из БД
                var dbScores = await _context.PlayerScores
                    .OrderByDescending(s => s.Score)
                    .Take(10)
                    .ToListAsync();

                if (dbScores.Any())
                {
                    HighScores = dbScores.Select(s => new PlayerScore
                    {
                        PlayerName = s.PlayerName,
                        Score = s.Score,
                        Date = s.Date
                    }).ToList();
                }
                else
                {
                    // Возвращаемся к хранящимся в памяти
                    HighScores = GameData.AppScores
                        .OrderByDescending(s => s.Score)
                        .Take(10)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading scores from database");

                // Возвращаемся к хранящимся в памяти
                HighScores = GameData.AppScores
                    .OrderByDescending(s => s.Score)
                    .Take(10)
                    .ToList();
            }

            // Если не получается, используем демо-заглушку
            if (!HighScores.Any())
            {
                InitializeDemoScores();
            }
        }

        private void InitializeDemoScores()
        {
            HighScores = new List<PlayerScore>();
            int baseScore = 1000;

            for (int i = 0; i < 10; i++)
            {
                HighScores.Add(new PlayerScore
                {
                    PlayerName = $"Игрок {i + 1}",
                    Score = baseScore - (i * 100),
                    Date = DateTime.Now.AddDays(-i)
                });
            }
        }

        public class PlayerScore
        {
            public string PlayerName { get; set; }
            public int Score { get; set; }
            public DateTime Date { get; set; }
        }
    }
}