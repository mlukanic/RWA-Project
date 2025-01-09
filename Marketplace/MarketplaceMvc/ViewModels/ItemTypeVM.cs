using System.ComponentModel.DataAnnotations;

namespace MarketplaceMvc.ViewModels
{
    public class ItemTypeVM
    {
        [Display(Name = "Item type")]
        [Required(ErrorMessage = "Enter something in here")]
        public int ItemTypeId { get; set; }

        [Display(Name = "Item type")]
        [Required(ErrorMessage = "Enter something in here")]
        public string? TypeName { get; set; }
    }
}
    