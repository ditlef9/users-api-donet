using Microsoft.AspNetCore.Mvc;

namespace UsersApiDotnet.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController(){

    }

    [HttpGet("test/{testValue}")]
    public string[] Test(string testValue)
    {
        string[] responseArray = new string[] {
            "test1",
            "test2"
        };
        return responseArray;
    }
}