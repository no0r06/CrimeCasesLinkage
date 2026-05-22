using CrimeCasesLinkage.Data;
using CrimeCasesLinkage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrimeCasesLinkage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class V4Controller : ControllerBase
    {
        private readonly CrimeDbContext _context;

        public V4Controller(CrimeDbContext context)
        {
            _context = context;
        }

        // -----------------------------
        // Normalization (same idea as V0/V1 style)
        // -----------------------------
        private string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "unknown";

            return input.Trim().ToLower();
        }

        // -----------------------------
        // V4 - Top-K + Input-Based Matching
        // -----------------------------
        [HttpPost("topk")]
        public async Task<IActionResult> CompareWithTopK(
            [FromBody] NewCaseInput input,
            [FromQuery] int threshold = 3,
            [FromQuery] int topK = 10)
        {
            var startTime = DateTime.Now;

            // Normalize input case
            var nLocation = Normalize(input.Location);
            var nMethod = Normalize(input.Method);
            var nGender = Normalize(input.VictimGender);

            var results = new List<object>();
            int comparisons = 0;

            // -----------------------------
            // STEP 1: Candidate filtering (indexed-friendly conditions)
            // -----------------------------
            var candidates = await _context.CrimeCases
                .AsNoTracking()
                .Include(c => c.Victims)
                .Where(c =>
                    c.Location.ToLower() == nLocation ||
                    c.Method.ToLower() == nMethod ||
                    c.Victims.Any(v => v.Gender.ToLower() == nGender)
                )
                .ToListAsync();

            // -----------------------------
            // STEP 2: Compute similarity + maintain Top-K
            // -----------------------------
            var topMatches = new List<(int Id, int Score)>();

            foreach (var crimeCase in candidates)
            {
                comparisons++;

                int score = CalculateSimilarity(input, crimeCase);

                if (score >= threshold)
                {
                    topMatches.Add((crimeCase.Id, score));

                    // keep only Top-K (dynamic pruning)
                    topMatches = topMatches
                        .OrderByDescending(x => x.Score)
                        .Take(topK)
                        .ToList();
                }
            }

            // -----------------------------
            // STEP 3: Format output
            // -----------------------------
            foreach (var match in topMatches)
            {
                var matchedCase = candidates.First(c => c.Id == match.Id);

                results.Add(new
                {
                    CaseId = matchedCase.Id,
                    SimilarityScore = match.Score,
                    Location = matchedCase.Location,
                    Method = matchedCase.Method
                });
            }

            return Ok(new
            {
                Algorithm = "V4 - Input-Based Top-K + Indexed Filtering",
                Input = new
                {
                    Location = nLocation,
                    Method = nMethod,
                    Gender = nGender
                },
                TotalCandidates = candidates.Count,
                TotalComparisons = comparisons,
                TopK = topK,
                Threshold = threshold,
                TimeMs = (DateTime.Now - startTime).TotalMilliseconds,
                Results = results
            });
        }

        // -----------------------------
        // Similarity Function (same logic family as V0-V3)
        // -----------------------------
        private int CalculateSimilarity(NewCaseInput input, CrimeCase c)
        {
            int score = 0;

            if (Normalize(c.Location) == Normalize(input.Location))
                score++;

            if (Normalize(c.Method) == Normalize(input.Method))
                score++;

            var victim = c.Victims.FirstOrDefault();

            if (victim != null)
            {
                if (Normalize(victim.Gender) == Normalize(input.VictimGender))
                    score++;

                bool ageOverlap =
                    input.VictimAgeMin <= victim.AgeMax &&
                    victim.AgeMin <= input.VictimAgeMax;

                if (ageOverlap)
                    score++;
            }

            return score;
        }
    }
}