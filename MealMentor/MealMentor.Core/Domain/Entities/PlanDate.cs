
namespace MealMentor.Core.Domain.Entities;

public partial class PlanDate
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreateDateTime { get; set; }

    public string? Accessibility { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<PlanDateDetail> PlanDateDetails { get; set; } = new List<PlanDateDetail>();
}
