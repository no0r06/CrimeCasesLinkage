namespace CrimeCasesLinkage.Models
{
    public static class LookupValues
    {
        public static List<string> Locations = new()
        {
            "Home", "Street", "Park", "Workplace", "Vehicle", "Unknown"
        };

        public static List<string> Methods = new()
        {
            "Stabbing", "Shooting", "Strangulation", "BluntForce", "Poison", "Unknown"
        };

        public static List<string> Genders = new()
        {
            "Male", "Female", "Unknown"
        };
    }
}