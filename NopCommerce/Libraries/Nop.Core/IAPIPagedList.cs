
using System.Collections.Generic;

namespace Nop.Core
{
    
    /// <summary>
    /// API Paged list interface
    /// </summary>
    public interface IAPIPagedList<T>
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
        int TotalCount { get; set; }
        int TotalPages { get; set; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        List<T> Items { get; set; }
    }
}
