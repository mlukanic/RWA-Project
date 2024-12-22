using System.ComponentModel.DataAnnotations;

namespace MarketplaceMvc.ViewModels
{
    public class TagVM
    {
        [Display(Name = "Tag")]
        [Required(ErrorMessage = "Enter something in here")]
        public int TagId { get; set; }

        [Display(Name = "Tag")]
        [Required(ErrorMessage = "Enter something in here")]
        public string? TagName { get; set; }
    }
}
