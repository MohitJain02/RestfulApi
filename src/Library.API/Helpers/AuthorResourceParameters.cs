using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class AuthorResourceParameters
    {
        const int maxPageSize = 20;

        public string OrderBy { get; set; } = "Name";

        public string Fields { get; set; }

        public string Genre { get; set; }

        public string SearchQuery { get; set; }

        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get
            {
                return pageSize;
            }

            set
            {
                pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
