namespace CrimeCasesLinkage.Models
{
    public class CrimeCase
    {
        public int Id { get; set; }
        public DateTime IncidentDate { get; set; }
        public string Location { get; set; }
        public string Method { get; set; }
        public List<Victim> Victims { get; set; }
    }
}