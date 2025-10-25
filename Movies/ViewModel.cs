using Movies.Models;

namespace Movies
{
    public class ViewModel
    {
        public Movie Movie { get; set; }
        public List<Actor> Actor  { get; set; }
        public List<int> SelectedActorIds { get; set; }
        public IFormFile MainImgFile { get; set; }
        public Category Category { get; set; }
    }
}
