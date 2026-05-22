using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrimeCasesLinkage.Data;
using CrimeCasesLinkage.Models;

namespace CrimeCasesLinkage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkageController : ControllerBase
    {
        private readonly CrimeDbContext _context;

        public LinkageController(CrimeDbContext context)
        {
            _context = context;
        }

        // GET: api/linkage/options
        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            return Ok(new
            {
                Locations = LookupValues.Locations,
                Methods = LookupValues.Methods,
                Genders = LookupValues.Genders
            });
        }

        // Normalize input
        private string NormalizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "unknown";

            return input.Trim().ToLower();
        }

        // =========================================
        // V0 - Naive Compare Everything
        // =========================================

        // POST: api/linkage/v0/naive?threshold=3
        [HttpPost("v0/naive")]
        public async Task<IActionResult> V0_Naive(
            [FromBody] NewCaseInput newCase,
            [FromQuery] int threshold = 3)
        {
            var startTime = DateTime.Now;

            var normalizedLocation = NormalizeInput(newCase.Location);
            var normalizedMethod = NormalizeInput(newCase.Method);
            var normalizedGender = NormalizeInput(newCase.VictimGender);

            var existingCases = await _context.CrimeCases
                .Include(c => c.Victims)
                .ToListAsync();

            var results = new List<object>();
            int totalComparisons = 0;

            foreach (var existing in existingCases)
            {
                totalComparisons++;

                int score = 0;

                // Location comparison
                if (NormalizeInput(existing.Location) == normalizedLocation)
                    score++;

                // Method comparison
                if (NormalizeInput(existing.Method) == normalizedMethod)
                    score++;

                var victim = existing.Victims.FirstOrDefault();

                if (victim != null)
                {
                    // Gender comparison
                    if (NormalizeInput(victim.Gender) == normalizedGender)
                        score++;

                    // Age overlap comparison
                    bool ageOverlap =
                        newCase.VictimAgeMin <= victim.AgeMax &&
                        victim.AgeMin <= newCase.VictimAgeMax;

                    if (ageOverlap)
                        score++;
                }

                if (score >= threshold)
                {
                    results.Add(new
                    {
                        CaseId = existing.Id,
                        SimilarityScore = score,
                        Location = existing.Location,
                        Method = existing.Method,
                        VictimGender = victim?.Gender ?? "Unknown",
                        VictimAgeRange = victim != null
                            ? $"{victim.AgeMin}-{victim.AgeMax}"
                            : "Unknown"
                    });
                }
            }

            var sortedResults = results
                .OrderByDescending(r => ((dynamic)r).SimilarityScore)
                .ToList();

            var endTime = DateTime.Now;
            var elapsedMs = (endTime - startTime).TotalMilliseconds;

            return Ok(new
            {
                Algorithm = "V0 - Naive",
                Threshold = threshold,
                TotalExistingCases = existingCases.Count,
                TotalComparisons = totalComparisons,
                PairsFound = results.Count,
                TimeMs = Math.Round(elapsedMs, 0),
                Results = sortedResults
            });
        }

        // =========================================
        // V1 - Grouped By Method
        // =========================================

        // POST: api/linkage/v1/grouped?threshold=3
        [HttpPost("v1/grouped")]
        public async Task<IActionResult> V1_Grouped(
            [FromBody] NewCaseInput newCase,
            [FromQuery] int threshold = 3)
        {
            var startTime = DateTime.Now;

            var normalizedLocation = NormalizeInput(newCase.Location);
            var normalizedMethod = NormalizeInput(newCase.Method);
            var normalizedGender = NormalizeInput(newCase.VictimGender);

            var allCases = await _context.CrimeCases
                .Include(c => c.Victims)
                .ToListAsync();

            // Group/filter by method first
            var existingCases = allCases
                .Where(c =>
                    c.Method != null &&
                    NormalizeInput(c.Method) == normalizedMethod)
                .ToList();

            var results = new List<object>();
            int totalComparisons = 0;

            foreach (var existing in existingCases)
            {
                totalComparisons++;

                int score = 0;

                // Method comparison
                score++;

                // Location comparison
                if (NormalizeInput(existing.Location) == normalizedLocation)
                    score++;

                var victim = existing.Victims.FirstOrDefault();

                if (victim != null)
                {
                    // Gender comparison
                    if (NormalizeInput(victim.Gender) == normalizedGender)
                        score++;

                    // Age overlap comparison
                    bool ageOverlap =
                        newCase.VictimAgeMin <= victim.AgeMax &&
                        victim.AgeMin <= newCase.VictimAgeMax;

                    if (ageOverlap)
                        score++;
                }

                if (score >= threshold)
                {
                    results.Add(new
                    {
                        CaseId = existing.Id,
                        SimilarityScore = score,
                        Location = existing.Location,
                        Method = existing.Method,
                        VictimGender = victim?.Gender ?? "Unknown",
                        VictimAgeRange = victim != null
                            ? $"{victim.AgeMin}-{victim.AgeMax}"
                            : "Unknown"
                    });
                }
            }

            var sortedResults = results
                .OrderByDescending(r => ((dynamic)r).SimilarityScore)
                .ToList();

            var endTime = DateTime.Now;
            var elapsedMs = (endTime - startTime).TotalMilliseconds;

            return Ok(new
            {
                Algorithm = "V1 - Grouped By Method",
                Threshold = threshold,

                InputMethod = normalizedMethod,

                TotalExistingCases = allCases.Count,
                TotalCasesInMethodGroup = existingCases.Count,
                TotalComparisons = totalComparisons,

                PairsFound = results.Count,

                TimeMs = Math.Round(elapsedMs, 0),

                Results = sortedResults
            });
        }
    }
}