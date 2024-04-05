using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared;


namespace Application.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IMongoQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken) where T : class
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            long totalItemCount = await source.LongCountAsync(cancellationToken);

            if (totalItemCount == 0)
            {
                return PaginatedResult<T>.Create(new List<T>(), 0, pageNumber, pageSize);
            }

            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return PaginatedResult<T>.Create(items, (int)totalItemCount, pageNumber, pageSize);
        }
    }
}
