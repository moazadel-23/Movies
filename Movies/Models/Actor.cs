using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Actor
    {
        [Key]
        public int Act_Id { get; set; }
        public string Name { get; set; } =string.Empty;
        public int Age { get; set; }
        [Required]
        public string Img { get; set; } = string.Empty;
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
