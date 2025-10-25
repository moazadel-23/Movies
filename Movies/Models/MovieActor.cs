using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    [PrimaryKey(nameof(Mov_Id), nameof(Act_Id))]
    public class MovieActor
    {
        [Key]
        public int Id { get; set; }
        public int Mov_Id { get; set; }
        public Movie? Movie { get; set; }
        public int Act_Id { get; set; }
        public Actor? Actor { get; set; }
    }
}
