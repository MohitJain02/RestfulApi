using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public class LinkDto
    {
        public string Href { get; set; }

        public string Method { get; set; }

        public string Rel { get; set; }

        public LinkDto(string Href, string Method, string Rel)
        {
            this.Href = Href;
            this.Method = Method;
            this.Rel = Rel;
        }
    }
}
