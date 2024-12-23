using Microsoft.AspNetCore.Mvc;
using UsersApiDotnet.Data;
using UsersApiDotnet.Dtos;
using UsersApiDotnet.Models;

namespace UsersApiDotnet.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config){
        _dapper = new DataContextDapper(config);
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection(){
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetUser/{userId}")]
    public User GetUser(int userId)
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM Users
            WHERE UserId = @UserId";

        var parameters = new { UserId = userId };
        User user = _dapper.LoadDataSingle<User>(sql, parameters);
        return user;
    }



    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
            UPDATE Users
            SET [FirstName] = @FirstName, 
                [LastName] = @LastName, 
                [Email] = @Email, 
                [Gender] = @Gender, 
                [Active] = @Active
            WHERE UserId = @UserId";
        
        var parameters = new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.Gender,
            user.Active,
            user.UserId
        };

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (@FirstName, @LastName, @Email, @Gender, @Active)";
        
        var parameters = new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            user.Gender,
            user.Active
        };

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }
    
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
            DELETE FROM Users 
            WHERE UserId = @UserId";
        
        var parameters = new { UserId = userId };

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }



    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(@"
            SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM  TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString());
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryForInsert.UserId.ToString()
                + ", " + userSalaryForInsert.Salary
                + ")";

        if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
        {
            return Ok(userSalaryForInsert);
        }
        throw new Exception("Adding User Salary failed on save");
    }

    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserSalary SET Salary=" 
            + userSalaryForUpdate.Salary
            + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryForUpdate);
        }
        throw new Exception("Updating User Salary failed on save");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId=" + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Deleting User Salary failed on save");
    }

    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
    {
        return _dapper.LoadData<UserJobInfo>(@"
            SELECT  UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM  TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId.ToString());
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                + ", '" + userJobInfoForInsert.Department
                + "', '" + userJobInfoForInsert.JobTitle
                + "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForInsert);
        }
        throw new Exception("Adding User Job Info failed on save");
    }

    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='" 
            + userJobInfoForUpdate.Department
            + "', JobTitle='"
            + userJobInfoForUpdate.JobTitle
            + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForUpdate);
        }
        throw new Exception("Updating User Job Info failed on save");
    }


    
    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();
        
        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        } 

        throw new Exception("Failed to Delete User");
    }
}