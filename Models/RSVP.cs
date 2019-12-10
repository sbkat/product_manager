using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace wedding_planner.Models
{
    public class RSVP
    {
        public int RSVPId { get; set; }
        public int UserId { get; set; }
        public int WeddingId { get; set; }
        public bool IsGoing { get; set; }
        [NotMapped]
        public List<User> Users { get; set; }
        public Wedding Wedding { get; set; }
    }
}