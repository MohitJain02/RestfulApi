using System;
using System.Collections.Generic;
using System.Linq;
using BuildingRestFullApi.Entities;

namespace BuildingRestFullApi.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;

        public LibraryRepository(LibraryContext context)
        {
            _context = context;
        }
        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(x => x.FirstName).ThenBy(x => x.LastName);
        }

        public Author GetAuthorById(Guid authorId)
        {
            return _context.Authors.FirstOrDefault(x => x.Id == authorId);
        }

        public IEnumerable<Author> GetAuthorsByIds(IEnumerable<Guid> authorIds)
        {
            return _context.Authors.Where(x => authorIds.Contains(x.Id)).OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName).ToList();
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);

            if (author.Books.Any())
            {
                foreach (var book in author.Books)
                {
                    book.Id = Guid.NewGuid();
                }
            }
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void UpdateAuthor(Author author)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorExists(Guid authorId)
        {
           return _context.Authors.Any(x => x.Id == authorId);
        }

        public IEnumerable<Book> GetBooksForAuthorById(Guid authorId)
        {
            return _context.Books.Where(x => x.AuthorId == authorId).OrderBy(x => x.Title).ToList();
        }

        public Book GetBookForAuthor(Guid authorId, Guid bookId)
        {
            return _context.Books.FirstOrDefault(x => x.AuthorId == authorId && x.Id == bookId);
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthorById(authorId);

            if (author != null)
            {
                if (book.Id == null)
                {
                    book.Id = Guid.NewGuid();
                }

                author.Books.Add(book);
            }
        }

        public void UpdateBookForAuthor(Book book)
        {
            throw new NotImplementedException();
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public bool SaveChanges()
        {
           return _context.SaveChanges() >=0 ;
        }
    }
}
