using System.Collections.Generic;

namespace Outline.Api.Extensions
{
    public class Pagination<T>
    {
        public int CurrentPage { get; set; }

        public int PageCount { get; set; }

        public int TotalItems { get; set; }

        public List<T> Result { get; set; }
    }
}