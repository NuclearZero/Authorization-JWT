using AuthorizationAPI.Services;
using Microsoft.EntityFrameworkCore;
using Models.ChargedHours;

namespace AuthorizationAPI.Repository
{
    public class UserRepository(IDbContextFactory<Models.ChargedHours.AedsystemContext> contextFactory, IPasswordHasher hasher) : IUserRepository
    {
        private readonly IDbContextFactory<Models.ChargedHours.AedsystemContext> _contextFactory = contextFactory;
        private readonly IPasswordHasher _hasher = hasher;

        public async Task<EmployeeTable> Get(string email, string password)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            using (var context = _contextFactory.CreateDbContext())
            {
                var user = await context.EmployeeTables.Where(u => u.EmployeeEmail == email)
                                                       .FirstOrDefaultAsync()
                                                       .ConfigureAwait(false);

                if (user == null)
                    return null;

                if (_hasher.VerifyPassword(user.PasswordHash, password))
                {
                    return user;
                }
                return null;
            }

        }

        public EmployeeTable Get_User(string email, string password)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.EmployeeTables.Where(u => u.EmployeeEmail != null && u.EmployeeEmail == email &&
                                                         u.PasswordHash != null && u.PasswordHash == password)
                                             .FirstOrDefault();
            }
        }

        public EmployeeTable Get_UserByRefreshToken(string refreshToken)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return context.EmployeeTables.Where(u => u.RefreshToken == refreshToken).FirstOrDefault();
            }
        }

        public bool Insert_User(string userName, string email, string password)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                if (context.EmployeeTables.Where(u => u.EmployeeEmail == email).FirstOrDefault() != null)
                    return false;
                try
                {
                    var hashedPassword = _hasher.Hash(password);
                    var newUser = new EmployeeTable()
                    {
                        FirstName = userName,
                        EmployeeEmail = email,
                        PasswordHash = hashedPassword
                    };
                    context.EmployeeTables.Add(newUser);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool Update_RefreshToken(EmployeeTable user, string refreshToken)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var userToChange = context.EmployeeTables.Where(u => u.Id == user.Id).FirstOrDefault();
                if (userToChange == null) return false;

                try
                {
                    userToChange.RefreshToken = refreshToken;
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<GroupTable> GetGroup_FromUser(EmployeeTable user)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.GroupTables.Where(g => g.Id == user.Group).FirstOrDefaultAsync();
            }
        }

        public async Task<TeamTable> GetTeam_FromUser(EmployeeTable user)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.TeamTables.Where(t => t.Id == user.Team).FirstOrDefaultAsync();
            }
        }
    }
}
