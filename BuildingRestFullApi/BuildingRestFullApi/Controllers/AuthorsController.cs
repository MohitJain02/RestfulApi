using System;
using System.Collections.Generic;
using BuildingRestFullApi.Entities;
using BuildingRestFullApi.Models;
using BuildingRestFullApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Utilities;

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

        [HttpGet("{id}", Name = "GetAuthorById")]

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

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorCreateDTO authorToCreate)
        {
            if (authorToCreate == null)
            {
                return BadRequest();
            }

            var authorToSave = AutoMapper.Mapper.Map<Author>(authorToCreate);

            _repository.AddAuthor(authorToSave);

            if (!_repository.SaveChanges())
            {
                throw new Exception("Not able to insert the author into the db");
            }

            var authorToReturn = AutoMapper.Mapper.Map<AuthorDTo>(authorToSave);
            // to return the newly created uri along with the 201 created status code
            return CreatedAtRoute("GetAuthorById", new {id = authorToSave.Id}, authorToReturn);
        }

        /// <summary>
        /// Just to return the correct status code when the post request is made for the particuler request type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_repository.IsAuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("id")]
        public IActionResult DeleteAuthor(Guid id)
        {

            var author = _repository.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }

            _repository.DeleteAuthor(author);

            if (!_repository.SaveChanges())
            {
                throw  new Exception($"Error while deleting the author with {id}");
            }

            return NoContent();
        }
    }
};