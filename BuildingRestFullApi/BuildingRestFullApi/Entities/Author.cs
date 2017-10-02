using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingRestFullApi.Entities
{
    /// <summary>
    /// This is an entity model and a complete replication of the author table of the library db
    /// </summary>
    public class Author
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTimeOffset DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        public string Genre { get; set; }

        public ICollection<Book> Books { get; set; }
            = new List<Book>();
    }
}
