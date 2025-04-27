using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Space_Invaders.Pages;

namespace Space_Invaders.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoresController : ControllerBase
    {
        private readonly SpaceInvaderDbContext _context;
        private readonly ILogger<ScoresController> _logger;

        public ScoresController(SpaceInvaderDbContext context, ILogger<ScoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ScoreData data)
        {
            if (string.IsNullOrWhiteSpace(data.PlayerName) || data.Score <= 0)
                return BadRequest("Invalid player name or score");

            try
            {
                // Try to save to database
                await SaveToDatabase(data.PlayerName, data.Score);

                // Update in-memory cache
                UpdateInMemoryScores(data.PlayerName, data.Score);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving score to database");

                // Fallback to in-memory storage
                UpdateInMemoryScores(data.PlayerName, data.Score);

                return Ok("Score saved locally (database unavailable)");
            }
        }

        private async Task SaveToDatabase(string playerName, int score)
        {
            // Check for existing score in DB
            var existingScore = await _context.PlayerScores
                .FirstOrDefaultAsync(s => s.PlayerName.ToLower() == playerName.ToLower());

            if (existingScore != null)
            {
                // Update only if new score is better
                if (score > existingScore.Score)
                {
                    existingScore.Score = score;
                    existingScore.Date = DateTime.UtcNow;
                    _context.PlayerScores.Update(existingScore);
                }
            }
            else
            {
                // Add new score
                _context.PlayerScores.Add(new PlayerScore
                {
                    PlayerName = playerName,
                    Score = score,
                    Date = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            // Remove scores outside top 10
            var allScores = await _context.PlayerScores
                .OrderByDescending(s => s.Score)
                .ToListAsync();

            if (allScores.Count > 10)
            {
                var scoresToRemove = allScores.Skip(10);
                _context.PlayerScores.RemoveRange(scoresToRemove);
                await _context.SaveChangesAsync();
            }
        }

        private void UpdateInMemoryScores(string playerName, int score)
        {
            // Find existing score (case insensitive)
            var existingScore = GameData.AppScores
                .FirstOrDefault(s => s.PlayerName.Equals(playerName, StringComparison.OrdinalIgnoreCase));

            if (existingScore != null)
            {
                // Update only if new score is better
                if (score > existingScore.Score)
                {
                    existingScore.Score = score;
                    existingScore.Date = DateTime.Now;
                }
            }
            else
            {
                // Add new score
                GameData.AppScores.Add(new ScoresModel.PlayerScore
                {
                    PlayerName = playerName,
                    Score = score,
                    Date = DateTime.Now
                });
            }

            // Keep only top 10 scores
            GameData.AppScores = GameData.AppScores
                .OrderByDescending(s => s.Score)
                .Take(10)
                .ToList();
        }

        public class ScoreData
        {
            public string PlayerName { get; set; }
            public int Score { get; set; }
        }
    }
}