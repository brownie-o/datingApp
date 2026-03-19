using System;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

// T is a generic type parameter that allows the class to handle any data type
public class PaginatedResult<T>
{
  public PaginationMetadata Metadata { get; set; } = default!; // ! could be null
  public List<T> Items { get; set; } = [];
};

public class PaginationMetadata
{
  public int CurrentPage { get; set; }
  public int TotalPages { get; set; }
  public int PageSize { get; set; }
  public int TotalCount { get; set; }
};

public class PaginationHelper
{
  // static method: so that we can use CreateAsync from PaginatedResult class without creating a new instance
  // Task<PaginatedResult<T>>: Async methods return a Task that returns a PaginatedResult of type T
  // IQueryable<T>: A query that has not executed yet. Allows EF Core to compose SQL
  public static async Task<PaginatedResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
  {
    // CountAsync(): allows us to get the total count of items in the query asynchronously
    var count = await query.CountAsync();
    // ToListAsync(): where the query actually goes to the database
    var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

    return new PaginatedResult<T>
    {
      Metadata = new PaginationMetadata
      {
        CurrentPage = pageNumber,
        TotalPages = (int)Math.Ceiling(count / (double)pageSize), // (double) for decimal division 10.0 instead of 10
        PageSize = pageSize,
        TotalCount = count
      },
      Items = items
    };
  }
}
