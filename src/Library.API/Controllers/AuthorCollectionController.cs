using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api/authorCollection")]
    public class AuthorCollectionController : Controller
    {
        private ILibraryRepository _libraryRepository;

        public AuthorCollectionController(ILibraryRepository libraryRepository)
        {
            this._libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection([FromBody]IEnumerable<AuthorForCreateDto> authorCollection)
        {
            if (authorCollection == null)
            {
                return BadRequest();
            }

            var authorEntities = Mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach(var author in authorEntities)
            {
                _libraryRepository.AddAuthor(author);
            }

            if(!_libraryRepository.Save())
            {
                throw new Exception("Exception while saving the bulk of authors");
            }

            var authorDtos = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsToString = string.Join(",", authorEntities.Select(x => x.Id));

            return CreatedAtRoute("GetAuthorCollections",
                                     new { ids = idsToString },
                                    authorDtos);
        }

        // (key1, key2, key3....) -- ArrayKeys
        // (key1 = value1, key2 = value2...) - composite keys
        [HttpGet("({ids})", Name = "GetAuthorCollections")]
        public IActionResult GetAuthorCollections(
            [ModelBinder(BinderType = typeof(ArrayModelBinders))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _libraryRepository.GetAuthors(ids);

            if(ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorDtos = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorDtos);
        }


        /// <summary>
        /// Scenerio where user post with the ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if(!_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }
         
    }
}
