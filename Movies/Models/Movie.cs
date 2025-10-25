using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Movie
    {
        [Key]
        public int Mov_Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool Status { get; set; }
        public DateTime Date { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public string SubImg { get; set; } = string.Empty;
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int CinemaId { get; set; }
        public Cinema? Cinema { get; set; }
    }
}
