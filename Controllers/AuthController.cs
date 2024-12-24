using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
                return BadRequest(new ApiResponse(false, "Invalid input data."));
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

            if (sqlExistingUser == null || !sqlExistingUser.Any())
            {
                // Generate a password salt
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                // Generate the password hash
                byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

                // Parameterized SQL query to insert data
                string sqlAddAuth = @"
                    INSERT INTO Auth ([Email], [PasswordHash], [PasswordSalt]) 
                    VALUES (@Email, @PasswordHash, @PasswordSalt)";
                var sqlParameters = new 
                {
                    Email = userForRegistration.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };
                if (_dapper.ExecuteSql(sqlAddAuth, sqlParameters))
                {
                    
                    string sqlAddUser = @"
                            INSERT INTO Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (@FirstName, @LastName, @Email, @Gender, @Active)";
                    var sqlParametersU = new 
                    {
                        FirstName = userForRegistration.FirstName,
                        LastName = userForRegistration.LastName,
                        Email = userForRegistration.Email,
                        Gender = userForRegistration.Gender,
                        Active = true
                    };

                    if (_dapper.ExecuteSql(sqlAddUser, sqlParametersU))
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to add user.");
                }
                throw new Exception("Failed to register user.");
            }
            throw new Exception("User with this email already exists!");
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

            return Ok();

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

    }
}