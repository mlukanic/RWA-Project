using System;
using System.Collections.Generic;

namespace MarketplaceClassLibrary.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DatePosted { get; set; }

    public string Condition { get; set; } = null!;

    public int ItemTypeId { get; set; }

    public virtual ItemType ItemType { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; } = new List<Reservation>();

    public virtual ICollection<ItemTag> ItemTags { get; } = new List<ItemTag>();
}
