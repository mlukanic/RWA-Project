namespace MarketplaceMvc.ViewModels
{
    public class ReservationUserVM
    {
        public string? Username { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public int FromPager { get; set; }
        public int ToPager { get; set; }
        public int LastPage { get; set; }
        public IEnumerable<ReservationVM>? Reservations { get; set; }
        public string? Submit { get; set; }
    }
}
