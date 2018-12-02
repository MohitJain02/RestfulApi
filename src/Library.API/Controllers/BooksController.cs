using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private ILibraryRepository _libraryRepository;

        private ILogger<BooksController> _logger;

        private IUrlHelper _urlHelper;

        public BooksController(
            ILibraryRepository libraryRepository,
            ILogger<BooksController> logger,
            IUrlHelper urlHelper)
        {
            this._libraryRepository = libraryRepository;
            this._logger = logger;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetBooks")]
        public IActionResult GetBooks(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var booksDto = Mapper.Map<IEnumerable<BooksDto>>(booksFromRepo);

            booksDto = booksDto.Select(
                book =>
                {
                    CreateLinksForBook(book);
                    return book;

                });

            var bookWrapper = new LinkedCollectionResourceWrapperDto<BooksDto>(booksDto);

            return Ok(CreateLinksForBooks(bookWrapper));
        }

        [HttpGet("{id}", Name = "GetBookByAuthorId")]
        public IActionResult GetBookByAuthorId(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRrpo = _libraryRepository.GetBookForAuthor(authorId, id);

            var bookDto = Mapper.Map<BooksDto>(bookFromRrpo);

            return Ok(CreateLinksForBook(bookDto));
        }

        [HttpPost(Name = "CreatBookByAuthor")]
        public IActionResult CreatBookByAuthor(Guid authorId, [FromBody]BookCreationDto bookCreationDto)
        {
            if (bookCreationDto == null)
            {
                return BadRequest();
            }

            if (bookCreationDto.Title == bookCreationDto.Description)
            {
                ModelState.AddModelError(nameof(BookCreationDto), "Title can not be same as the description of the book");
            }

            if (!ModelState.IsValid)
            {
                // return 422
                return new UnProcessibleEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookEntity = Mapper.Map<Book>(bookCreationDto);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Error while saving the new Book for the author with the id : {authorId}");
            }

            var bookToReturn = Mapper.Map<BooksDto>(bookEntity);

            return CreatedAtRoute("GetBookByAuthorId", new
            {
                authorId = bookToReturn.AuthorId,
                id = bookToReturn.Id
            }, CreateLinksForBook(bookToReturn));
        }

        [HttpDelete("{id}", Name = "DeleteBookForAuthor")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();

            }

            var bookForAuthor = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthor == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(bookForAuthor);

            if (_libraryRepository.Save())
            {
                throw new Exception($"Error for author with id {authorId} while deleting the book with id :{id}");
            }

            _logger.LogInformation(100, $"Book with the authorId: {authorId} deleted successfully");

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateBookForAuthor")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] UpdateBookDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (book.Title == book.Description)
            {
                ModelState.AddModelError(nameof(UpdateBookDto), "title can not be same as description");
            }

            if (!ModelState.IsValid)
            {
                return new UnProcessibleEntityObjectResult(ModelState);
            }

            var bookForAuthor = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthor == null)
            {
                var bookToAdd = Mapper.Map<Book>(book);
                bookToAdd.Id = id;
                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                {
                    throw new Exception($"Error while saving the upserting Book for the author with the authorid : {authorId}");
                }

                var bookToReturn = Mapper.Map<BooksDto>(bookToAdd);

                return CreatedAtRoute("GetBookByAuthorId", new
                {
                    authorId = bookToReturn.AuthorId,
                    id = bookToReturn.Id
                }, bookToReturn);
            }

            Mapper.Map(book, bookForAuthor); // when we map then the values are being updated in the entity model

            _libraryRepository.UpdateBookForAuthor(bookForAuthor);

            if (!_libraryRepository.Save())
            {
                throw new Exception($"Error while updating the book with id {id} for author with id : {authorId}");

            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBookForAuthor")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,
            [FromBody] JsonPatchDocument<UpdateBookDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorForBookRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (authorForBookRepo == null)
            {
                // Perform upserting on Path

                var bookToInsert = new UpdateBookDto();
                patchDocument.ApplyTo(bookToInsert, ModelState);

                if (bookToInsert.Title == bookToInsert.Description)
                {
                    ModelState.AddModelError(nameof(UpdateBookDto), "Book Title and Description can not be same");
                }

                TryValidateModel(bookToInsert);

                if (!ModelState.IsValid)
                {
                    return new UnProcessibleEntityObjectResult(ModelState);
                }

                var newBook = Mapper.Map<Book>(bookToInsert);
                newBook.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, newBook);
                if (_libraryRepository.Save())
                {
                    throw new Exception($"Error for book with id {id} for the author id :{authorId} while creating it");
                }

                var bookToReturn = Mapper.Map<BooksDto>(newBook);
                return CreatedAtRoute("GetBookByAuthorId",
                    new
                    {
                        authorId,
                        id = bookToReturn.Id
                    },
                        bookToReturn);
            }

            var bookToPatch = Mapper.Map<UpdateBookDto>(authorForBookRepo);

            patchDocument.ApplyTo(bookToPatch, ModelState); // this will validate only the patched docx


            if (bookToPatch.Title == bookToPatch.Description)
            {
                ModelState.AddModelError(nameof(UpdateBookDto), "Title and Description can not be same");
            }


            // to validate the updateDto inside the doc

            TryValidateModel(bookToPatch);


            if (!ModelState.IsValid)
            {
                return new UnProcessibleEntityObjectResult(ModelState);
            }

            // add validations

            Mapper.Map(bookToPatch, authorForBookRepo);

            _libraryRepository.UpdateBookForAuthor(authorForBookRepo);

            if (_libraryRepository.Save())
            {
                throw new Exception($"Error while updating the book for the author with bookId :{id}");
            }

            return NoContent();
        }

        private BooksDto CreateLinksForBook(BooksDto book)
        {
            book.Links.Add(new LinkDto(
                _urlHelper.Link("GetBookByAuthorId", new { id = book.Id }),
                "self",
                "Get"));

            book.Links.Add(new LinkDto(
                _urlHelper.Link("DeleteBookForAuthor", new { id = book.Id }),
            "delete_book",
            "DELETE"));

            book.Links.Add(new LinkDto(
                _urlHelper.Link("UpdateBookForAuthor", new { id = book.Id }),
                "update_book",
                "PUT"));

            book.Links.Add(new LinkDto(
                _urlHelper.Link("PartiallyUpdateBookForAuthor", new { id = book.Id }),
                "patially_update_book",
                "PATCH"));

            return book;
        }

        private LinkedCollectionResourceWrapperDto<BooksDto> CreateLinksForBooks(LinkedCollectionResourceWrapperDto<BooksDto> bookWrapper)
        {
            bookWrapper.Links.Add(new LinkDto(
                _urlHelper.Link("GetBooks", new { }),
                "self",
                "GET"));

            return bookWrapper;
        }

    }
}
