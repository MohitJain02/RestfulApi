using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public class AuthorForCreateWithDateOfDeathDto
    {
        public string Genre { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public DateTimeOffset DateOfDeath { get; set; }


        public string FirstName { get; set; }

        public string LastName { get; set; }

        public ICollection<BookCreationDto> Books { get; set; }
           = new List<BookCreationDto>();
    }
}
