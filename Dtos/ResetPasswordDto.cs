using System.ComponentModel.DataAnnotations;

namespace Project.Dtos
{
    public class ResetPasswordDto
    {

     [Required]
    public string Id { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    
    [Required]
    public string code { get; set; }
    }
}