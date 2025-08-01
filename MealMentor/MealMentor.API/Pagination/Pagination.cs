using Microsoft.EntityFrameworkCore;
using static MealMentor.Core.DTOs.PaginationDTO.PaginationModel;

namespace MealMentor.API.Pagination
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, PaginationParams paginationParams)
        {
            var result = new PagedResult<T>
            {
                CurrentPage = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = await query.CountAsync()
            };

            result.Items = await query.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                                      .Take(paginationParams.PageSize)
                                      .ToListAsync();

            return result;
        }
    }
}
