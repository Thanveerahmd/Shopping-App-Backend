using System.ComponentModel.DataAnnotations;

namespace pro.backend.Dtos
{
    public class UpdateUserDto
    {
        [Required]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string imageUrl { get; set; }
    }

}