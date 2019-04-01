using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Project.Entities;

namespace Project.Data
{
    
    public static class Seed
    {
         // private readonly RoleManager<Role> _roleManager;
        //  public Seed(RoleManager<Role> roleManager){
        //     _roleManager = roleManager;  
        // }
        // public void SeedRoles()
        // {
        
        // var roles = new List<Role>
        //     {
        //         new Role{Name="Customer"},
        //         new Role{Name="Seller"},
        //         new Role{Name="Modarater"},
        //     };
        // foreach (var role in roles)
        // {
        //     _roleManager.CreateAsync(role).Wait();
        // }
        // } 
  public static void SeedRoles(RoleManager<Role> roleManager)
{
    if (!roleManager.RoleExistsAsync("Customer").Result)
    {
        Role role = new Role();
        role.Name ="Customer";
        role.discs = "Perform Customer operations.";
        IdentityResult roleResult = roleManager.
        CreateAsync(role).Result;
    }


    if (!roleManager.RoleExistsAsync("Seller").Result)
    {
        Role role = new Role();
        role.Name = "Seller";
        role.discs = "Perform all the operations.";
        IdentityResult roleResult = roleManager.
        CreateAsync(role).Result;
    }

    if (!roleManager.RoleExistsAsync("Moderator").Result)
    {
        Role role = new Role();
        role.Name ="Moderator";
        role.discs = "Perform both customer and seller operations.";
        IdentityResult roleResult = roleManager.
        CreateAsync(role).Result;
    }

}
    }
}