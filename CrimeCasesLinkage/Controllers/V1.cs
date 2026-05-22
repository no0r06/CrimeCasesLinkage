using CrimeCasesLinkage.Data;
using CrimeCasesLinkage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrimeCasesLinkage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class V1Controller : ControllerBase
    {
        private readonly CrimeDbContext _context;

        public V1Controller(CrimeDbContext context)
        {
            _context = context;
        }

        private string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "unknown";
            return input.Trim().ToLower();
        }

        // POST: api/v1/compare?threshold=3
        [HttpPost("compare")]
        public async Task<IActionResult> Compare([FromBody] NewCaseInput input, [FromQuery] int threshold = 3)
        {
            var startTime = DateTime.Now;

            var method = Normalize(input.Method);
            var location = Normalize(input.Location);
            var gender = Normalize(input.VictimGender);

            var cases = await _context.CrimeCases
                .Include(c => c.Victims)
                .ToListAsync();

            var results = new List<object>();
            int comparisons = 0;

            foreach (var c in cases)
            {
                comparisons++;
                int score = 0;

                // WEIGHTS (V1: METHOD IS MOST IMPORTANT)
                if (Normalize(c.Method) == method) score += 3;
                if (Normalize(c.Location) == location) score += 1;

                var v = c.Victims.FirstOrDefault();
                if (v != null)
                {
                    if (Normalize(v.Gender) == gender) score += 1;

                    bool ageOverlap =
                        input.VictimAgeMin <= v.AgeMax &&
                        v.AgeMin <= input.VictimAgeMax;

                    if (ageOverlap) score += 1;
                }

                if (score >= threshold)
                {
                    results.Add(new
                    {
                        CaseId = c.Id,
                        Score = score,
                        Method = c.Method,
                        Location = c.Location
                    });
                }
            }

            return Ok(new
            {
                Algorithm = "V1 - Method Weighted Model",
                TotalCases = cases.Count,
                TotalComparisons = comparisons,
                Results = results.OrderByDescending(r => ((dynamic)r).Score),
                TimeMs = (DateTime.Now - startTime).TotalMilliseconds
            });
        }
    }
}