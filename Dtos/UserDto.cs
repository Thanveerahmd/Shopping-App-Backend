namespace Project.Dtos
{
    public class UserDto
    {
        
         public string UserRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public  string Email { get; set; }
        public string Password { get; set; }
         public string Role { get; set; }

         public string Image {get; set; }
        public bool IsSocialMedia { get; set; }
    }
}