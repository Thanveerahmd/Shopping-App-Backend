using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace  Project.Entities
{
    public class User : IdentityUser
    {    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string imageUrl { get; set; }
    }
}