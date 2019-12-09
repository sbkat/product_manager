namespace wedding_planner.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int WeddingId { get; set; }
        public User User { get; set; }
        public Wedding Wedding { get; set; }
    }
}