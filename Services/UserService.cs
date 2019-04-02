using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using  Project.Dtos;
using  Project.Entities;
using Project.Helpers;
using  Project.Services;

namespace Project.Services
{
    public class UserService : IUserService
    {
         private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public User AuthenticateUser(string username, string password, bool IsSocialMedia)
        {
            if (!IsSocialMedia)
            {

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return null;

                var user = _context.Users.FirstOrDefault(x => x.UserName == username);

                // check if username exists
                if (user == null)
                    return null;

                // check if password is correct
                // if (!Encoder.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                //     return null;

                // authentication successful
                return user;

            }
            else
            {

                if (string.IsNullOrEmpty(username))
                    return null;

                var user = _context.Users.FirstOrDefault(x => x.UserName == username);

                // check if username exists
                if (user == null)
                {
                  return null;

                }

                // authentication successful
                return user;
            }

        }

        public IEnumerable<User> GetAllUser()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

         public User GetById(string id)
        {
            return _context.Users.Find(id);
        }

        // public User RegisterUser(User user, string password)
        // {
        //     if (!user.IsSocialMedia)
        //     {
        //         // validation
        //         if (string.IsNullOrWhiteSpace(password))
        //             throw new AppException("Password is required");

        //         if (_context.Users.Any(x => x.UserName == user.UserName))
        //             throw new AppException("Username \"" + user.UserName + "\" is already taken");

        //         byte[] passwordHash, passwordSalt;
        //         Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);

        //         // user.PasswordHash = passwordHash;
        //         // user.PasswordSalt = passwordSalt;

        //         _context.Users.Add(user);
        //         _context.SaveChanges();

        //         return user;
        //     }
        //     else
        //     {

        //         if (_context.Users.Any(x => x.UserName == user.UserName))
        //             throw new AppException("Username \"" + user.UserName + "\" is already taken");

        //         _context.Users.AddAsync(user);
        //         _context.SaveChangesAsync();

        //         return user;
        //     }

        // }

        // public void UpdateUser(User userParam, string password = null)
        // {
        //     var user = _context.Users.Find(userParam.Id);

        //     if (user == null)
        //         throw new AppException("User not found");

        //     if (userParam.UserName != user.UserName)
        //     {
        //         // username has changed so check if the new username is already taken
        //         if (_context.Users.Any(x => x.UserName == userParam.UserName))
        //             throw new AppException("Username " + userParam.UserName + " is already taken");
        //     }
        //     // update user properties
        //     user.FirstName = userParam.FirstName;
        //     user.LastName = userParam.LastName;
        //     user.UserName = userParam.UserName;
        //     // update password if it was entered
        //     if (!string.IsNullOrWhiteSpace(password))
        //     {
        //         byte[] passwordHash, passwordSalt;
        //         Encoder.CreatePasswordHash(password, out passwordHash, out passwordSalt);
        //         // user.PasswordHash = passwordHash;
        //         // user.PasswordSalt = passwordSalt;
        //     }
        //     _context.Users.Update(user);
        //     _context.SaveChanges();
        // }
        // public void DeleteUser(int id)
        // {
        //     var user = _context.Users.Find(id);
        //     if (user != null)
        //     {
        //         _context.Users.Remove(user);
        //         _context.SaveChanges();
        //     }
        // }

        // private helper methods

        

       

        
    }
}