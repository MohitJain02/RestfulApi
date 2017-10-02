using System.Collections.Generic;
using BuildingRestFullApi.Entities;
using BuildingRestFullApi.Models;
using BuildingRestFullApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BuildingRestFullApi.Controllers
{
    [Route("api/authorscollection")]
    public class AuthorsCollectionController : Controller
    {
        private ILibraryRepository _repository;

        public AuthorsCollectionController(ILibraryRepository libraryRepository)
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

            return Ok();
        }
    }
}
