namespace CrimeCasesLinkage.Models
{
    public class Victim
    {
        public int Id { get; set; }
        public int AgeMin { get; set; }
        public int AgeMax { get; set; }
        public string Gender { get; set; }

        public int CrimeCaseId { get; set; }

        public CrimeCase? CrimeCase { get; set; }
    }
}
