using System.ComponentModel.DataAnnotations;

namespace Project.Dtos
{
    public class ForgetPasswordDto
    {
     [Required]
    public string Email { get; set; }
    
    
    }
}