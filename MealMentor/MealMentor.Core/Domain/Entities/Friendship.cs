using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class Friendship
{
    public int Id { get; set; }

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public int? Status { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? ResponseDate { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User? Sender { get; set; }
}
