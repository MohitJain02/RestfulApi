using System.Collections.Generic;

namespace Library.API.Models
{
    // Base class for implementing HATEOAS
    public abstract class LinkedResourceBaseDto
    {
        public List<LinkDto> Links { get; set; }
            = new List<LinkDto>();
    }
}
