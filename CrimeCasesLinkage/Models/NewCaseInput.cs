namespace CrimeCasesLinkage.Models
{
    public class NewCaseInput
    {
        public string Location { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string VictimGender { get; set; } = string.Empty;
        public int VictimAgeMin { get; set; }
        public int VictimAgeMax { get; set; }
    }
}