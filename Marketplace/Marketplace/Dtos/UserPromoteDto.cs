using System.ComponentModel.DataAnnotations;

namespace Marketplace.Dtos
{
    public class UserPromoteDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string Username
        {
            get; set;
        }
    }
}
