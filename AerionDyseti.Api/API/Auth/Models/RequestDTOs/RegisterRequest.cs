using System.ComponentModel.DataAnnotations;

namespace AerionDyseti.API.Auth.Models.RequestDTOs
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}
