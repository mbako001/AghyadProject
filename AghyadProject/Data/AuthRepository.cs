using AghyadProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AghyadProject.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthRepository(AppDbContext context , IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            var response = new ServiceResponse<string>();
            if (user == null)
            {
                response.Success = false;
                response.Message = "user is not found";
            }
            user.PasswordResetToken = CreateToken(user);
            user.RestTokenExpires = DateTime.Now.AddDays(1);
            response.Data = user.PasswordResetToken;
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(username));
            if (user == null)
            {
                response.Success = false;
                response.Message = "User is not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash , user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
            {
                response.Data = CreateToken(user);
            }
            return response;
        }

        public async Task<ServiceResponse<string>> Register
            (User user , string email , string password)
        {
            var response = new ServiceResponse<string>();
            if(await UserExists(user.UserName))
            {
                response.Success = false;
                response.Message = "User is already exists.";
                return response;
            }

            CreatePassordHash(password, out byte[] passowrdHash, out byte[] passowrdSalt);
            user.PasswordHash = passowrdHash;
            user.PasswordSalt = passowrdSalt;
            user.Email = email;
            user.VerificationToken = CreateToken(user);
            response.Data = user.VerificationToken;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return response;    
        }

        public async Task<ServiceResponse<string>> ResetPassowrd(string Token , string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == Token);
            if(user == null)
            {
                response.Success = false;
                response.Message = "Invalid Token.";
                return response; 
            }
            CreatePassordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.RestTokenExpires = null;
            user.PasswordResetToken = null;
            return response;
        }

        public async Task<bool> UserExists(string userName)
        {
            if(await _context.Users.AnyAsync(u => u.UserName.ToLower() == userName))
            {
                return true;
            }
            return false;
        }
        private void CreatePassordHash(string password , out byte[] passwordHash , out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));    
            } 
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Email)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("jwt:Signingkey").Value));
            SigningCredentials creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        private bool VerifyPasswordHash(string passowrd , byte[] passowrdHash , byte[] passowrdSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passowrdSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passowrd));
                return computeHash.SequenceEqual(passowrdHash);
            }
        }
    }
}
