using System;

namespace StarWarsAPI.Application.DTOs
{
    public class CreateMovieDto
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public string Producer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int EpisodeId { get; set; }
        public string OpeningCrawl { get; set; }
    }
}
