using System;
using System.Collections.Generic;

namespace BuildingRestFullApi.Models
{
    /// <summary>
    /// This model is used for sending the request to the server
    /// </summary>
    public class AuthorCreateDTO
    {
        public string FirstName  { get; set; }

        public string LastName { get; set; }

        public string Genre { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }

        public IEnumerable<BookCreateDTO> Books { get; set; }= new List<BookCreateDTO>();
    }
}
