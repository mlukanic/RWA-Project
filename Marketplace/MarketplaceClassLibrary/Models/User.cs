using System;
using System.Collections.Generic;

namespace MarketplaceClassLibrary.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PwdHash { get; set; } = null!;

    public string PwdSalt { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Reservation> Reservations { get; } = new List<Reservation>();

    public virtual Role Role { get; set; } = null!;
}
