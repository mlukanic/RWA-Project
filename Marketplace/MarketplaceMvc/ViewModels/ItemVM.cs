using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MarketplaceMvc.ViewModels
{
    public class ItemVM
    {
        public int ItemId { get; set; }

        [Display(Name = "Item type")]
        [Required(ErrorMessage = "Enter something in here")]
        public int? ItemTypeId { get; set; }

        [Display(Name = "Item Type")]
        public string? TypeName { get; set; }

        [Required(ErrorMessage = "Enter something in here")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Enter something in here")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Enter something in here")]
        public string? Condition { get; set; }

        [ValidateNever]
        public List<int>? TagIds { get; set; }

        [ValidateNever]
        public List<TagVM>? Tags { get; set; }

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
