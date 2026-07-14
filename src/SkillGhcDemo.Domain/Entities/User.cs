using SkillGhcDemo.Domain.Common;

namespace SkillGhcDemo.Domain.Entities;

/// <summary>
/// Represents a user (customer/buyer) of the store. Can have a history
/// of orders (<see cref="Order"/>).
/// </summary>
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
