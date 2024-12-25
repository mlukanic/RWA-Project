using System;
using System.Collections.Generic;

namespace MarketplaceClassLibrary.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public int ItemId { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
