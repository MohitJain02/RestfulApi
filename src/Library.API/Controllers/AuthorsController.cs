using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private ILibraryRepository _libraryRepository;
        private IUrlHelper _urlHelper;
        private IPropertyMappingService _propertyMappingService;
        private ITypeHelperService _typeHelperService;

        public AuthorsController(ILibraryRepository libraryRepository,
                                 IUrlHelper urlHelper,
                                 IPropertyMappingService propertyMappingService,
                                 ITypeHelperService typeHelperService)
        {
            this._libraryRepository = libraryRepository;
            this._urlHelper = urlHelper;
            this._propertyMappingService = propertyMappingService;
            this._typeHelperService = typeHelperService;
        }

        protected override 

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(
            [FromQuery] AuthorResourceParameters authorResourceParameters,
            [FromHeader(Name = "Accept")] string header)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>
                (authorResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProperties<AuthorDto>(authorResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = _libraryRepository.GetAuthors(authorResourceParameters);

            if (header.Equals("application/vnd.marvin.hateoas+json"))
            {
                var paginationMetadata = new
                {
                    totalCount = authorsFromRepo.TotalCount,
                    totalPages = authorsFromRepo.TotalPages,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));


                var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);

                var links = CreateLinksForAuthors(authorResourceParameters, authorsFromRepo.HasPrevious, authorsFromRepo.HasNext);

                var authorShapedOject = authors.ShapeData(authorResourceParameters.Fields);

                var shapedAuthorLinks = authorShapedOject.Select(author =>
                {
                    var authorAsDictionary = author as IDictionary<string, object>;

                    var authorLink = CreateLinksForAuthor(
                                                (Guid)authorAsDictionary["Id"],
                                                authorResourceParameters.Fields);

                    authorAsDictionary.Add("links", authorLink);

                    return authorAsDictionary;
                });

                var linkedAuthorToReturn = new
                {
                    links,
                    value = shapedAuthorLinks
                };

                return Ok(linkedAuthorToReturn); 
            }
            else
            {
                var previousLink = authorsFromRepo.HasPrevious ?
                    CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.PreviousPage)
                    : null;

                var nextLink = authorsFromRepo.HasNext ?
                    CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.NextPage)
                    : null;

                var paginationMetadata = new
                {
                    previousLink,
                    nextLink,
                    totalCount = authorsFromRepo.TotalCount,
                    totalPages = authorsFromRepo.TotalPages,
                    pageSize = authorsFromRepo.PageSize,
                    currentPage = authorsFromRepo.CurrentPage
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
                var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
                return Ok(authors.ShapeData(authorResourceParameters.Fields));
            }
        }

        private string CreateAuthorsResourceUri(
             AuthorResourceParameters authorResourceParameters,
             ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors", new
                    {
                        orderBy = authorResourceParameters.OrderBy,
                        fields = authorResourceParameters.Fields,
                        searchQuery = authorResourceParameters.SearchQuery,
                        genre = authorResourceParameters.Genre,
                        pageNumber = authorResourceParameters.PageNumber - 1,
                        pageSize = authorResourceParameters.PageSize

                    });

                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors", new
                    {

                        orderBy = authorResourceParameters.OrderBy,
                        fields = authorResourceParameters.Fields,
                        searchQuery = authorResourceParameters.SearchQuery,
                        genre = authorResourceParameters.Genre,
                        pageNumber = authorResourceParameters.PageNumber + 1,
                        pageSize = authorResourceParameters.PageSize
                    });

                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetAuthors", new
                    {

                        orderBy = authorResourceParameters.OrderBy,
                        fields = authorResourceParameters.Fields,
                        searchQuery = authorResourceParameters.SearchQuery,
                        genre = authorResourceParameters.Genre,
                        pageNumber = authorResourceParameters.PageNumber,
                        pageSize = authorResourceParameters.PageSize
                    });
            }
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id, [FromQuery]string fields)
        {

            if (!_typeHelperService.TypeHasProperties<AuthorDto>(fields))
            {
                return BadRequest();
            }

            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo != null)
            {
                var author = Mapper.Map<AuthorDto>(authorFromRepo);

                var links = CreateLinksForAuthor(id, fields);

                var linksToReturn = author.ShapeData(fields)
                                      as IDictionary<string, object>;

                linksToReturn.Add("links", links);

                return Ok(linksToReturn);
            }
            else
            {
                return NotFound($"Author with id: {id} not found");
            }
        }

        [HttpPost(Name = "CreateAuthor")]
        [RequestHeaderMatchesMediaTypeHeader("Content-Type", new[] { "application/vnd.marvin.author.full+json" })]
        public IActionResult CreateAuthor([FromBody] AuthorForCreateDto authorForCreateDto)
        {
            // the frombody model is not serialized
            // properly then its a badRequest
            if (authorForCreateDto == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(authorForCreateDto);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Exception while creating the author");
            }
            var authorToDisplay = Mapper.Map<AuthorDto>(authorEntity);

            var links = CreateLinksForAuthor(authorToDisplay.Id, null);

            var linkedToReturn = authorToDisplay.ShapeData(null) as IDictionary<string, object>;

            linkedToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedToReturn["Id"] }, linkedToReturn);
        }

        [HttpPost(Name = "CreateAuthorWithDateOfDeath")]
        [RequestHeaderMatchesMediaTypeHeader("Content-Type", new[] { "application/vnd.marvin.authorwithdateofdeath.full+json",
                                                                      "application/vnd.marvin.authorwithdateofdeath.full+xml"})]
        [RequestHeaderMatchesMediaTypeHeader("Accept", new[] { "..." })]

        public IActionResult CreateAuthorWithDateOfDeath([FromBody] AuthorForCreateWithDateOfDeathDto authorForCreateDto)
        {
            // the frombody model is not serialized
            // properly then its a badRequest
            if (authorForCreateDto == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(authorForCreateDto);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Exception while creating the author");
            }
            var authorToDisplay = Mapper.Map<AuthorDto>(authorEntity);

            var links = CreateLinksForAuthor(authorToDisplay.Id, null);

            var linkedToReturn = authorToDisplay.ShapeData(null) as IDictionary<string, object>;

            linkedToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { id = linkedToReturn["Id"] }, linkedToReturn);
        }

        [HttpDelete(Name = "DeleteForAuthor")]
        public IActionResult DeleteForAuthor(Guid id)
        {
            var authorEntity = _libraryRepository.GetAuthor(id);

            if (authorEntity == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorEntity);

            if (_libraryRepository.Save())
            {
                throw new Exception($"Error while deleting the author with the id {id}");
            }

            return NoContent();
        }

        private IEnumerable<LinkDto> CreateLinksForAuthor(Guid id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrEmpty(fields))
            {
                links.Add(new LinkDto(
                    _urlHelper.Link("GetAuthors", new { id }),
                    "self",
                    "GET"));
            }

            else
            {
                links.Add(new LinkDto(
                    _urlHelper.Link("GetAuthor", new { id, fields }),
                    "self",
                    "GET"));
            }

            links.Add(new LinkDto(
                _urlHelper.Link("DeleteForAuthor", new { id }),
                      "delete_for_author",
                       "DELETE"));

            links.Add(new LinkDto(
                _urlHelper.Link("CreatBookByAuthor", new { authorId = id }),
                "crete_book_for_author",
                "POST"));

            links.Add(new LinkDto(
                _urlHelper.Link("GetBooks", new { authorId = id }),
                "get_book_for_author",
                "GET"));
            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorResourceParameters authorResourceParameters,
            bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>
            {
                new LinkDto(
                CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.Current),
                "self",
                "GET")
            };
            if (hasNext)
            {
                links.Add(new LinkDto(
                    CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.NextPage),
                    "nexPage",
                    "GET"));
            }

            if(hasPrevious)
            {
                links.Add(new LinkDto(
                    CreateAuthorsResourceUri(authorResourceParameters, ResourceUriType.PreviousPage),
                    "previousPage",
                    "GET"));
            }
            return links;
        }
    }
}
