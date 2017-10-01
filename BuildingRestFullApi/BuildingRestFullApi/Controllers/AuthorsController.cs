using System;
using System.Collections.Generic;
using BuildingRestFullApi.Models;
using BuildingRestFullApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BuildingRestFullApi.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _repository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _repository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {

            try
            {
                //throw new Exception("Unhandled  Excption just to reproduce 500 error");
                var authorResult = _repository.GetAuthors();

                var authorDTO = AutoMapper.Mapper.Map<IEnumerable<AuthorDTo>>(authorResult);

                // return new JsonResult(authorDTO);

                return Ok(authorDTO);
            }
            catch (Exception)
            {

                return StatusCode(500, "Thrown an 500 fault");
            }
        }

        [HttpGet("{id}")]

        public IActionResult GetAuthorById(Guid id)
        {
            var author = _repository.GetAuthorById(id);

            if (author == null)
            {
                return NotFound();
            }

            var matchedAuthor = AutoMapper.Mapper.Map<AuthorDTo>(author);

            //return new JsonResult(matchedAuthor);

            return Ok(matchedAuthor);
        }
    }
}