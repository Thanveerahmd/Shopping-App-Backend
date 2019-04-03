using System.ComponentModel.DataAnnotations;

namespace pro.backend.Dtos
{
    public class AdminPasswordResetDto
    {
       
        public int Id { get; set; }
         [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

         public string ActivationCode { get; set; }
    }
}