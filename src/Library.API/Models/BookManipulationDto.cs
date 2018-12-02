using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public abstract class BookManipulationDto
    {
        [Required(ErrorMessage = "Title is Required to be added")]
        [MaxLength(100, ErrorMessage = "Max length can not exceed 100")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "Max length can not exceed more then 500")]
        public virtual string Description { get; set; }
    }
}
