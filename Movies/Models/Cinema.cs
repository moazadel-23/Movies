using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class Cinema
    {
        [Key]
        public int Cin_Id { get; set; }
        public string Img { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? ImgFile { get; set; }
    }
}
