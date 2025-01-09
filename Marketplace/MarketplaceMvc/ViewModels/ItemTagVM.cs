using System.ComponentModel.DataAnnotations;

namespace MarketplaceMvc.ViewModels
{
    public class ItemTagVM
    {
        public int ItemTagId { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "Enter something in here")]
        public int? ItemId { get; set; }

        [Display(Name = "Item")]
        public string? ItemTitle { get; set; }

        [Display(Name = "Tag")]
        [Required(ErrorMessage = "Enter something in here")]
        public int? TagId { get; set; }

        [Display(Name = "Tag")]
        public string? TagName { get; set; }
    }
}
