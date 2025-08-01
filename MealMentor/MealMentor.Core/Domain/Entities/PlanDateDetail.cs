using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class PlanDateDetail
{
    public int Id { get; set; }

    public string? Meal { get; set; }

    public int? PlanId { get; set; }

    public string? Type { get; set; }

    public DateTime? PlanTime { get; set; }

    public int? Number { get; set; }

    public virtual PlanDate? Plan { get; set; }
}
