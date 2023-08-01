using System;
using System.Collections.Generic;
using System.Linq;

namespace Mebeller.Data.Utilities;

public class Paging<T>
{
    public Paging(IEnumerable<T> query, int pageSize, int pageNumber = 1)
    {
        var queryCount = query.Count();
        var totalPages = (int)Math.Ceiling(decimal.Divide(queryCount, pageSize));
        FirstPage = 1;
        LastPage = totalPages;
        PreviousPage = Math.Max(pageNumber - 1, FirstPage);
        NextPage = Math.Min(pageNumber + 1, LastPage);
        QueryResult = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public IEnumerable<T> QueryResult { get; }
    public int FirstPage { get; }
    public int LastPage { get; }
    public int PreviousPage { get; }
    public int NextPage { get; }
}