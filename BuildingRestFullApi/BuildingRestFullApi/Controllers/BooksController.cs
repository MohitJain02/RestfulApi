using System;
using System.Collections.Generic;
using BuildingRestFullApi.Entities;
using BuildingRestFullApi.Models;
using BuildingRestFullApi.Services;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Utilities;

namespace BuildingRestFullApi.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _repository;

        public BooksController(ILibraryRepository repository)
        {
            _repository = repository;   
        }

        [HttpGet]
        public IActionResult GetBookForAuthors(Guid authorId)
        {
            if (!_repository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthors = _repository.GetBooksForAuthorById(authorId);

            var booksResult = AutoMapper.Mapper.Map<IEnumerable<BookDTO>>(booksForAuthors);

            return Ok(booksResult);
        }

        [HttpGet("{id}" , Name = "GetBookByIdForAuthors")]
        public IActionResult GetBookByIdForAuthors(Guid authorId, Guid id)
        {
            if (!_repository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var authorForBooks = _repository.GetBookForAuthor(authorId, id);
            if (authorForBooks == null)
            {
                return NotFound();
            }

            var singleBookResult = AutoMapper.Mapper.Map<BookDTO>(authorForBooks);

            return Ok(singleBookResult);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorid, [FromBody] BookCreateDTO bookToCreate)
        {
            if (bookToCreate == null)
            {
                return BadRequest();
            }

            if (!_repository.IsAuthorExists(authorid))
            {
                return NotFound();
                
            }

            var bookToSave = AutoMapper.Mapper.Map<Book>(bookToCreate);

            _repository.AddBookForAuthor(authorid, bookToSave);

            var bookToReturn = AutoMapper.Mapper.Map<BookDTO>(bookToSave);

            return CreatedAtRoute("GetBookByIdForAuthors", new
            {
                authorId = authorid,
                id = bookToSave.Id

            }, bookToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_repository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var authorForBooks = _repository.GetBookForAuthor(authorId, id);
            if (authorForBooks == null)
            {
                return NotFound();
            }
            _repository.DeleteBook(authorForBooks);
            if (!_repository.SaveChanges())
            {
                throw  new Exception($"Error while deleting the book with {authorId} & {id} ");
            }

            return NoContent();
        }
    }
}
