using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class Category
    {
        [Key]
        public int Cat_Id { get; set; }
        public int Status { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
