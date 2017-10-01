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

        [HttpGet("{id}")]
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
    }
}
