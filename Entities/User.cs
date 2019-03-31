using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace  Project.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserRole { get; set; }
         public bool IsSocialMedia { get; set; }
        
    }
}