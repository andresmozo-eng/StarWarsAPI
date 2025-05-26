using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarWarsAPI.Domain.Entities;

namespace StarWarsAPI.Infrastructure.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.EpisodeId)
                .IsRequired();

            builder.Property(f => f.OpeningCrawl)
                .HasColumnType("text");

            builder.Property(f => f.Director)
                .HasMaxLength(100);

            builder.Property(f => f.Producer)
                .HasMaxLength(200);

            builder.Property(f => f.ReleaseDate)
                .IsRequired();
        }
    }
}
