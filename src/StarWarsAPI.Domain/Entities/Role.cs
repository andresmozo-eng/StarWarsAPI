using System.Collections.Generic;

namespace StarWarsAPI.Domain.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
