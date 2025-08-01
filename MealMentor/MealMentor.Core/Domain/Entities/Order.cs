using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int? OrderCode { get; set; }

    public string? UserId { get; set; }

    public DateTime? CreatedDateAt { get; set; }

    public string? Status { get; set; }

    public double? Price { get; set; }

    public virtual User? User { get; set; }
}
