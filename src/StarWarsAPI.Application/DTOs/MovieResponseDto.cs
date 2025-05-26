using System;

namespace StarWarsAPI.Application.DTOs
{
    public class MovieResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
        public string Producer { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string OpeningCrawl { get; set; }
    }
}
