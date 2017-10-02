using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingRestFullApi.Models
{
    /// <summary>
    /// This model is used to receive the response from the server
    /// </summary>
    public class AuthorDTo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Genre { get; set; }
    }
}
