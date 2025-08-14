using Microsoft.AspNetCore.Identity;

namespace AuthorizationAPI.Services
{
    public class EncryptPassword : IPasswordHasher
    {
        private PasswordHasher<string> _passwordHasher;

        public EncryptPassword()
        {
            _passwordHasher = new PasswordHasher<string>();
        }

        public string Hash(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashPassword, string verifiedPassword)
        {
            try
            {
                return _passwordHasher.VerifyHashedPassword(null, hashPassword, verifiedPassword) == 0 ? false : true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
