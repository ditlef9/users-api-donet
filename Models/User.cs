namespace UsersApiDotnet.Models;
public partial class User{
    public int UserId {get; set;}
    public String FirstName {get; set;}
    public String LastName {get; set;}
    public String Email {get; set;}
    public String Gender {get; set;}
    public bool Active {get; set;}
    
    public User(){
        if(FirstName == null){
            FirstName = "";
        }
        if(LastName == null){
            LastName = "";
        }
        if(Email == null){
            Email = "";
        }
        if(Gender == null){
            Gender = "";
        }
    }

}
