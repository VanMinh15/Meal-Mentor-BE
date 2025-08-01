using System;
using System.Collections.Generic;

namespace MealMentor.Core.Domain.Entities;

public partial class Ingredient
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? TranslatedName { get; set; }

    public string? Description { get; set; }

    public string? Nutrient { get; set; }

    public double? Quantity { get; set; }

    public string? Measure { get; set; }

    public string? FoodMatch { get; set; }

    public string? Food { get; set; }

    public string? FoodId { get; set; }

    public double? Weight { get; set; }

    public double? RetainedWeight { get; set; }

    public string? Url { get; set; }

    public int? IsDeleted { get; set; }
}
