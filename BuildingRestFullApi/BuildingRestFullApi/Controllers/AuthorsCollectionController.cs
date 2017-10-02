using System;
using System.Collections.Generic;
using BuildingRestFullApi.Entities;
using BuildingRestFullApi.Models;
using BuildingRestFullApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BuildingRestFullApi.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller
    {
        private ILibraryRepository _repository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _repository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollection(IEnumerable<AuthorCreateDTO> authorCollection)
        {
            if (authorCollection == null)
            {
                return BadRequest();
            }

            var authorCollectionToSave = AutoMapper.Mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach (var author in authorCollectionToSave)
            {
                _repository.AddAuthor(author);
            }

            if (_repository.SaveChanges())
            {
                throw new Exception("Creating an author collection fails");
            }

            return Ok();
        }
    }
}
