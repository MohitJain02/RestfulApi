﻿using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.API.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;

        private IPropertyMappingService _propertyMappingService;

        public LibraryRepository(LibraryContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);

            // the repository fills the id (instead of using identity columns)
            if (author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                // if there isn't an id filled out (ie: we're not upserting),
                // we should generate one
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(Guid authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public PagedList<Author> GetAuthors(AuthorResourceParameters authorResourceParameters)
        {
            //var collectionBeforePaging = _context.Authors
            //                             .OrderBy(a => a.FirstName)
            //                             .ThenBy(a => a.LastName).AsQueryable();

            var collectionBeforePaging = _context.Authors.ApplySort(authorResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<AuthorDto, Author>());

            if(!string.IsNullOrEmpty(authorResourceParameters.Genre))
            {
                var genreFoWhereClause = authorResourceParameters.Genre.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging.Where(x => x.Genre.ToLowerInvariant() == genreFoWhereClause);
            }

            if(!string.IsNullOrEmpty(authorResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = authorResourceParameters.SearchQuery.Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                                        .Where(x => x.Genre.ToLowerInvariant().Contains(searchQueryForWhereClause)
                                           || x.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                                           || x.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)); 
            }

            return PagedList<Author>.Create(collectionBeforePaging,
                                            authorResourceParameters.PageNumber,
                                            authorResourceParameters.PageSize);
                
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books
              .Where(b => b.AuthorId == authorId && b.Id == bookId).FirstOrDefault();
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public void UpdateBookForAuthor(Book book)
        {
            // no code in this implementation
            // as EF tracks the entity using the dbContext so
            // there is no need to implement this method if we are updating
            // the values using the automapper 
            // then calling save will persist those changes inside db
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
