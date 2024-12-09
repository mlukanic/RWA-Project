using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceClassLibrary.Models
{
    public partial class ItemTag
    {
        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; } = null!;
    }
}
