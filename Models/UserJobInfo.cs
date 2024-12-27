namespace UsersApiDotnet.Models;
public partial class UserJobInfo{
    public int UserId {get; set;}
    public String JobTitle {get; set;}
    public String Department {get; set;}
    
    public UserJobInfo(){
        if(JobTitle == null){
            JobTitle = "";
        }
        if(Department == null){
            Department = "";
        }
    }

}
