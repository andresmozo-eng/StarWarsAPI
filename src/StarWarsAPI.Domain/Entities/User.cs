using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace StarWarsAPI.Domain.Entities
{
    public class User
    {
        public User(string userName, string email, byte[] passwordHash, byte[] passwordSalt, int roleId)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            PasswordSalt = passwordSalt;
            RoleId = roleId;
        }
        private User() { }// Constructor necesario para EF

        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public byte[] PasswordHash { get; private set; }
        public byte[] PasswordSalt { get; private set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public void SetPassword(string password)
        {
            using var hmac = new HMACSHA512();
            PasswordSalt = hmac.Key;
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
