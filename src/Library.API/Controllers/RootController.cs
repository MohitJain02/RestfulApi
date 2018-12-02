using Library.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    /// <summary>
    /// To get the root level links 
    /// For the consumer
    /// </summary>
    [Route("api")]
    public class RootController : Controller
    {
        private IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet ( Name = "GetRootDocument")]
        public IActionResult GetRootDocument([FromHeader(Name = "Accept")] string mediaType)
        {
            if(mediaType == "application/vnd.marvin.hateoas+json")
            {
                var links = new List<LinkDto>();

                links.Add(new LinkDto(
                    _urlHelper.Link("GetRootDocument", new { }),
                    "self",
                    "GET"));

                links.Add(new LinkDto(
                    _urlHelper.Link("GetAuthors", new { }),
                    "get_author",
                    "GET"));

                links.Add(new LinkDto(
                    _urlHelper.Link("CreateAuthor", new { }),
                    "create_author",
                    "POST"));

                return Ok(links);
            }

            return NoContent();
        }
    }
}
