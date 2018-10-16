using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;
        public BooksController(ILibraryRepository libraryRepository)
        {
            this._libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetBooks(Guid authorId)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var booksDto = Mapper.Map<IEnumerable<BooksDto>>(booksFromRepo);

            return Ok(booksDto);
        }

        [HttpGet("{id}", Name = "GetBookByAuthorId")]
        public IActionResult GetBookByAuthorId(Guid authorId, Guid id)
        {
            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRrpo = _libraryRepository.GetBookForAuthor(authorId, id);

            var bookDto = Mapper.Map<BooksDto>(bookFromRrpo);

            return Ok(bookDto);
        }

        [HttpPost]
        public IActionResult CreatBookByAuthor(Guid authorId, [FromBody]BookCreationDto bookCreationDto)
        {
            if(bookCreationDto == null)
            {
                return BadRequest();
            }

            if(!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = Mapper.Map<Book>(bookCreationDto);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if(!_libraryRepository.Save())
            {
                throw new Exception($"Error while saving the new Book for the author with the id : {authorId}");
            }

            var bookToReturn = Mapper.Map<BooksDto>(bookEntity);

            return CreatedAtRoute("GetBookByAuthorId", new
            {
                authorId = bookToReturn.AuthorId,
                id = bookToReturn.Id
            }, bookToReturn);
        }

    }
}
