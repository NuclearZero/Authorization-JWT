using AuthorizationAPI.Repository;
using AuthorizationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController(ILogger<AuthorizeController> logger,
                                     IUserRepository userRepository,
                                     IJwtTokenGenerator jwtTokenGenerator) : Controller
    {
        private readonly ILogger<AuthorizeController> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

        [HttpPost]
        public async Task<IActionResult> Login([FromQuery] string password, string email)
        {
            // Tiene que devolver el JWT token
            var user = _userRepository.Get(email, password).Result;

            if (user == null) return Unauthorized("Invalid credentials");

            var group = await _userRepository.GetGroup_FromUser(user);
            var team = await _userRepository.GetTeam_FromUser(user);
            var userType = user.UserType;

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.EmployeeEmail),
                new Claim(CustomClaims.Group, group.Description),
                new Claim(CustomClaims.UserType, userType),
                new Claim(CustomClaims.Team, team.TeamName)
            };

            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var login = new Login()
            {
                AccessToken = _jwtTokenGenerator.GenerateAccessToken(claims),
                RefreshToken = refreshToken,
                User = user,
            };

            // Insertar el refresh token en el record del usuario
            _userRepository.Update_RefreshToken(user, refreshToken);

            return Ok(login);
        }

        [HttpPost]
        [Route("RefreshJwt")]
        public IActionResult RefreshToken([FromBody]Login login)
        {
            if (login == null) return BadRequest("Invalid client request");

            string accessToken = login.AccessToken;
            string refreshToken = login.RefreshToken;

            var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(accessToken);
            var dbUser = _userRepository.Get_UserByRefreshToken(login.RefreshToken);

            if (principal == null)
                return BadRequest("Invalid client request");

            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var newLogin = new Login()
            {
                User = dbUser,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            if (!_userRepository.Update_RefreshToken(login.User, newRefreshToken)) return Unauthorized();

            return Ok(newLogin);
        }
    }
}
