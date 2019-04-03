using System.Collections.Generic;
using System.Linq;
using Project.Entities;
using Project.Helpers;

namespace Project.Services
{
    public class AdminServices : iAdminServices
    {
        private DataContext _context;

        public AdminServices(DataContext context)
        {
            _context = context;
        }

        public Admin AddAdmin(Admin admin, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Admins.Any(x => x.Username == admin.Username))
                throw new AppException("Username \"" + admin.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);

            admin.PasswordHash = passwordHash;
            admin.PasswordSalt = passwordSalt;
            admin.FirstLogin = true;

            _context.Admins.Add(admin);
            _context.SaveChanges();

            return admin;
        }

        public Admin AuthenticateUser(string username, string password)

        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var admin = _context.Admins.FirstOrDefault(x => x.Username == username);

            // check if username exists
            if (admin == null)
                return null;

            // check if password is correct
            if (!Encoder.VerifyPasswordHash(password, admin.PasswordHash, admin.PasswordSalt))
                return null;

            // authentication successful
            return admin;
        }

        public void DeleteAdmin(int id)
        {
            var user = _context.Admins.Find(id);
            if (user != null)
            {
                _context.Admins.Remove(user);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Admin> GetAllAdmins()
        {
            return _context.Admins;
        }

        public Admin GetById(int id)
        {
            return _context.Admins.Find(id);
        }

        public void UpdateAdmin(Admin admin,string password)
        {
            var user = _context.Admins.Find(admin.Id);

            if (user == null)
                throw new AppException("User not found");

            if (admin.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Admins.Any(x => x.Username == admin.Username))
                    throw new AppException("Username " + admin.Username + " is already taken");
            }

            // update user properties
            user.FirstName = admin.FirstName;
            user.LastName = admin.LastName;
            user.Username = admin.Username;
            user.ActivationCode = admin.ActivationCode;
            user.IsEmailConfirmed = admin.IsEmailConfirmed;
            user.FirstLogin=admin.FirstLogin;


            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                
                user.FirstLogin = false;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Admins.Update(user);
            _context.SaveChanges();
        }

        public Admin GetByEmail(string id)
        {
            try{
                var admin = _context.Admins.Single(a => a.Username == id);
                return admin;
                }
            catch{
                return null;
            }
            

        }

        public void UpdateAdminPassword(Admin admin, string password)
        {
            var user = GetByEmail(admin.Username);

            if (user == null)
                throw new AppException("User not found");

            if (admin.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Admins.Any(x => x.Username == admin.Username))
                    throw new AppException("Username " + admin.Username + " is already taken");
            }


            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);
                
                user.FirstLogin = false;
                user.ActivationCode = null;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            _context.Admins.Update(user);
            _context.SaveChanges();
        }
    }
}