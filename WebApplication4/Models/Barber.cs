namespace WebApplication4.Models
{
    public class Stylist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string ImageUrl { get; set; }
        public List<string> AvailableTimes { get; set; } = new List<string>();
    }
}
