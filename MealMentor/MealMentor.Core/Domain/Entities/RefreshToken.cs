using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class RefreshToken
{
    public int TokenId { get; set; }

    public string UserId { get; set; } = null!;

    public string? Token { get; set; }

    public DateTime ExpirationDate { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
