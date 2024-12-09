using System;
using System.Collections.Generic;

namespace MarketplaceClassLibrary.Models;

public partial class Log
{
    public int Idlog { get; set; }

    public string? Message { get; set; }

    public string? Level { get; set; }

    public DateTime Timestamp { get; set; }
}
