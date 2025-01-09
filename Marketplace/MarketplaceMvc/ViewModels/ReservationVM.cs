using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceMvc.ViewModels
{
    public class ReservationVM
    {
        public int ReservationId { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Enter something in here")]
        public string? Username { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Enter something in here")]
        public int? ItemId { get; set; }

        [Display(Name = "Item")]
        public string? ItemTitle { get; set; }

        [ValidateNever]
        public int Page { get; set; } = 1;

        [ValidateNever]
        public int Size { get; set; } = 10;

        [ValidateNever]
        public int FromPager { get; set; }

        [ValidateNever]
        public int ToPager { get; set; }

        [ValidateNever]
        public int LastPage { get; set; }
    }
}
