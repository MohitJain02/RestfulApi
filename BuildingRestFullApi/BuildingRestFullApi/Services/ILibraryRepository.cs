using System;
using System.Collections.Generic;
using BuildingRestFullApi.Entities;

namespace BuildingRestFullApi.Services
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors();
        Author GetAuthorById(Guid authorId);
        IEnumerable<Author> GetAuthorsByIds(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool IsAuthorExists(Guid authorId);
        IEnumerable<Book> GetBooksForAuthorById(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid bookId);
        void AddBookForAuthor(Guid authorId, Book book);
        void UpdateBookForAuthor(Book book);
        void DeleteBook(Book book);
        bool SaveChanges();
    }
}
