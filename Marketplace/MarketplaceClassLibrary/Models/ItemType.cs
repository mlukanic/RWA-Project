using System;
using System.Collections.Generic;

namespace MarketplaceClassLibrary.Models;

public partial class ItemType
{
    public int ItemTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Item> Items { get; } = new List<Item>();
}
