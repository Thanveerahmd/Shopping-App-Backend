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

    
        
    }
}