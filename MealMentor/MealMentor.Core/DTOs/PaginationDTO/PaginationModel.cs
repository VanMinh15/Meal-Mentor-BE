﻿namespace MealMentor.Core.DTOs.PaginationDTO
{
    public class PaginationModel
    {
        public class PagedResult<T>
        {
            public List<T> Items { get; set; }
            public int TotalCount { get; set; }
            public int PageSize { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        }

        public class PaginationParams
        {
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }
    }
}
