using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int TotalCount { get; private set; }

        public int PageSize { get; private set; }

        public bool HasPrevious
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }

        public PagedList(List<T> totalItems, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalCount = count;
            PageSize = PageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(totalItems);
        }

        public static PagedList<T> Create(IQueryable<T> items, int pageNumber, int pageSize)
        {
            int count = items.Count();
            var totalItems = items.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            return new PagedList<T>(totalItems, count, pageNumber, pageSize);
        }
    }
}
