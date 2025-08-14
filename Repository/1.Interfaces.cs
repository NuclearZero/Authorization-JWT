using Models.ChargedHours;

namespace AuthorizationAPI.Repository
{
    public interface IUserRepository
    {
        Task<EmployeeTable> Get(string email, string password);
        EmployeeTable Get_User(string email, string password);
        EmployeeTable Get_UserByRefreshToken(string refreshToken);
        bool Insert_User(string userName, string email, string password);
        bool Update_RefreshToken(EmployeeTable user, string refreshToken);

        // Puede necesitar refactorizarse en el futuro
        Task<GroupTable> GetGroup_FromUser(EmployeeTable user);
        Task<TeamTable> GetTeam_FromUser(EmployeeTable user);
    }
}
