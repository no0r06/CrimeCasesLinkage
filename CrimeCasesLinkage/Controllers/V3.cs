using CrimeCasesLinkage.Data;
using CrimeCasesLinkage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrimeCasesLinkage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class V3Controller : ControllerBase
    {
        private readonly CrimeDbContext _context;

        public V3Controller(CrimeDbContext context)
        {
            _context = context;
        }

        private string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "unknown";

            return input.Trim().ToLower();
        }

        [HttpPost("indexed")]
        public async Task<IActionResult> CompareIndexed(
            [FromBody] NewCaseInput input,
            [FromQuery] int threshold = 3)
        {
            var startTime = DateTime.Now;

            var nLocation = Normalize(input.Location);
            var nGender = Normalize(input.VictimGender);
            var nMethod = Normalize(input.Method);

            // 🔥 STEP 1: DATABASE FILTER FIRST (NO FULL LOAD)
            var candidateCases = await _context.CrimeCases
                .AsNoTracking()
                .Include(c => c.Victims)
                .Where(c =>
                    c.Location.ToLower() == nLocation ||
                    c.Method.ToLower() == nMethod ||
                    c.Victims.Any(v => v.Gender.ToLower() == nGender)
                )
                .Take(2000) // prevents explosion
                .ToListAsync();

            var results = new List<object>();
            int comparisons = 0;

            // 🔥 STEP 2: ONLY COMPARE SMALL SET
            for (int i = 0; i < candidateCases.Count; i++)
            {
                for (int j = i + 1; j < candidateCases.Count; j++)
                {
                    comparisons++;

                    var a = candidateCases[i];
                    var b = candidateCases[j];

                    int score = 0;

                    if (a.Location.ToLower() == b.Location.ToLower())
                        score++;

                    if (a.Method.ToLower() == b.Method.ToLower())
                        score++;

                    var va = a.Victims.FirstOrDefault();
                    var vb = b.Victims.FirstOrDefault();

                    if (va != null && vb != null)
                    {
                        if (va.Gender.ToLower() == vb.Gender.ToLower())
                            score++;

                        bool ageOverlap =
                            va.AgeMin <= vb.AgeMax &&
                            vb.AgeMin <= va.AgeMax;

                        if (ageOverlap)
                            score++;
                    }

                    if (score >= threshold)
                    {
                        results.Add(new
                        {
                            CaseIdA = a.Id,
                            CaseIdB = b.Id,
                            Score = score
                        });
                    }
                }
            }

            return Ok(new
            {
                Algorithm = "V3 - TRUE Indexed DB Filtering + Reduced Search Space",
                CandidateCases = candidateCases.Count,
                Comparisons = comparisons,
                TimeMs = (DateTime.Now - startTime).TotalMilliseconds,
                Results = results
            });
        }
    }
}