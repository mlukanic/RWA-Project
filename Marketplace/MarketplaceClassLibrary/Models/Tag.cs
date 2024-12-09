using System;
using System.Collections.Generic;

namespace MarketplaceClassLibrary.Models;

public partial class Tag
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<ItemTag> ItemTags { get; } = new List<ItemTag>();
}
