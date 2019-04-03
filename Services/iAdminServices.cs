using System.Collections.Generic;
using Project.Entities;

namespace Project.Services
{
    public interface iAdminServices
    {
        Admin AuthenticateUser(string username, string password);
        IEnumerable<Admin> GetAllAdmins();
        Admin GetById(int id);
        Admin AddAdmin(Admin admin, string password);
        void UpdateAdmin(Admin admin);
        Admin GetByEmail(string id);
        void DeleteAdmin(int id);
        void UpdateAdminPassword(Admin admin, string password);
    }
}