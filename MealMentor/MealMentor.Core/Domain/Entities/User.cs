using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class User
{
    public string Id { get; set; } = null!;

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public double? Height { get; set; }

    public double? Weight { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Status { get; set; }

    public string? RecipeList { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual ICollection<Friendship> FriendshipReceivers { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipSenders { get; set; } = new List<Friendship>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PlanDate> PlanDates { get; set; } = new List<PlanDate>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
