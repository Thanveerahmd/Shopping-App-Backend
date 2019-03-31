using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Project.Entities
{
    public class Role : IdentityRole
    {
        public string discs { get; set; }
    }
}