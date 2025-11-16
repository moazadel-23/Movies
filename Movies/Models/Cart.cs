namespace Movies.Models
{
    public class Cart
    {
        public int Mov_Id { get; set; }
        public Movie Movie { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
        public int TicketCount { get; set; }
    }
}
