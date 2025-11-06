using Movies.Models;

namespace Movies.ViewModel
{
    public class MovieVM
    {
        public Movie Movie { get; set; } = new Movie();
        public List<Actor>? Actor  { get; set; }
        public List<int>? SelectedActorIds { get; set; }
        public IFormFile? MainImgFile { get; set; }
        public Category? Category { get; set; }
    }
}
