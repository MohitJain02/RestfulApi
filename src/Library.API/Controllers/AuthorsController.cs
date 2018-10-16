using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;
        public AuthorsController(ILibraryRepository libraryRepository)
        {
            this._libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = _libraryRepository.GetAuthors();

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

            return Ok(authors);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {

            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo != null)
            {
                var author = Mapper.Map<AuthorDto>(authorFromRepo);

                return Ok(author);
            }
            else
            {
                return NotFound($"Author with id: {id} not found");
            }
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreateDto authorForCreateDto)
        {
            // the frombody model is not serialized
            // properly then its a badRequest
            if(authorForCreateDto == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(authorForCreateDto);

            _libraryRepository.AddAuthor(authorEntity);

            if(!_libraryRepository.Save())
            {
                throw new Exception("Exception while creating the author");
            }
            var authorToDisplay = Mapper.Map<AuthorDto>(authorEntity);
            return  CreatedAtRoute("GetAuthor", new { id = authorToDisplay.Id }, authorToDisplay);
        }
    }
}
