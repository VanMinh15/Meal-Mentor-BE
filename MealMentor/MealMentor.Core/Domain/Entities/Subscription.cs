using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class Subscription
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int PlanId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime LastUpdated { get; set; }

    public virtual User User { get; set; } = null!;
}
