using System;

namespace StarWarsAPI.Domain.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public int EpisodeId { get; set; }
        public string Title { get; set; }
        public string OpeningCrawl { get; set; }
        public string Director { get; set; }
        public string Producer { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
