using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UsersApiDotnet.Data;
using UsersApiDotnet.Dtos;

namespace UsersApiDotnet.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        /* Register -----------------------------------------------------------------------*/
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(401, "Invalid input data.");
            }

            Console.WriteLine($"AuthController()·Register()·Registration Payload: {JsonConvert.SerializeObject(userForRegistration)}");


            Console.WriteLine($"AuthController()·Register()·Registration Payload: {Newtonsoft.Json.JsonConvert.SerializeObject(userForRegistration)}");
            if (string.IsNullOrWhiteSpace(userForRegistration.Email))
            {
                throw new Exception("Email cannot be empty.");
            }

            if (userForRegistration.Password != userForRegistration.PasswordConfirm)
            {
                throw new Exception("Passwords do not match!");
            }


            string sqlCheckUserExists = "SELECT Email FROM Auth WHERE Email = @Email";

            var parameters = new { Email = userForRegistration.Email };

            IEnumerable<string> sqlExistingUser = _dapper.LoadData<string>(sqlCheckUserExists, parameters);

            Console.WriteLine($"AuthController()·Register()·SQL Query: {sqlCheckUserExists}");
            Console.WriteLine($"AuthController()·Register()·Email Parameter: {userForRegistration.Email}");
            Console.WriteLine($"AuthController()·Register()·Query Results: {string.Join(", ", sqlExistingUser)}");

            if (sqlExistingUser != null && sqlExistingUser.Any())
            {
                throw new Exception("User with this email already exists!");
            }

            // Generate password salt
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            // Generate password hash
            byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

            // Insert authentication data
            string sqlAddAuth = @"
                INSERT INTO Auth ([Email], [PasswordHash], [PasswordSalt]) 
                VALUES (@Email, @PasswordHash, @PasswordSalt)";
            var sqlAuthParameters = new
            {
                Email = userForRegistration.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            if (!_dapper.ExecuteSql(sqlAddAuth, sqlAuthParameters))
            {
                throw new Exception("Failed to register user.");
            }

            // Insert user data
            string sqlAddUser = @"
                INSERT INTO Users (
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
                ) VALUES (@FirstName, @LastName, @Email, @Gender, @Active)";
            var sqlUserParameters = new
            {
                FirstName = userForRegistration.FirstName,
                LastName = userForRegistration.LastName,
                Email = userForRegistration.Email,
                Gender = userForRegistration.Gender,
                Active = true
            };

            if (!_dapper.ExecuteSql(sqlAddUser, sqlUserParameters))
            {
                throw new Exception("Failed to add user.");
            }

            return Ok();
        }


        /*- Login ------------------------------------------------------------------ */
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            // Use parameterized query
            string sqlForHashAndSalt = @"
                SELECT 
                    [PasswordHash],
                    [PasswordSalt] 
                FROM Auth 
                WHERE Email = @Email";
            var sqlParameters = new { Email = userForLogin.Email };

            // Execute the query securely using the parameter
            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

            if (userForConfirmation == null)
            {
                return StatusCode(401, "User not found!");
            }

            // Generate the password hash based on the provided password and stored salt
            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            // Compare the generated hash with the stored hash byte-by-byte
            if (!passwordHash.SequenceEqual(userForConfirmation.PasswordHash))
            {
                return StatusCode(401, "Incorrect password!");
            }

            string userIdSql = @"
                SELECT UserId FROM Users WHERE Email = '" +
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);
            return Ok(new Dictionary<string, string> {
                {"token", CreateToken(userId)}
            });

        }

        /*- Get Password Hash ------------------------------------------------------ */
        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

        /*- Create Token --------------------------------------------------------- */
        private string CreateToken(int userId)
        {
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials(
                    tokenKey,
                    SecurityAlgorithms.HmacSha512Signature
                );

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);

        }
        
        /*- Refresh Token --------------------------------------------------------- */
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return CreateToken(userId);
        }

    }
}