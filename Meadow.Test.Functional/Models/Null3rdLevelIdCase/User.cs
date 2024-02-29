using Acidmanic.Utilities.Reflection.Attributes;

namespace Meadow.Test.Functional.Models.Null3rdLevelIdCase;


[OwnerName("Users")]
public class UserStorage
{
    [UniqueMember]
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string MobileNumber { get; set; }
    
    public string NationalId { get; set; }
    
    public string Email { get; set; }
    
    public string ProfilePictureId { get; set; }
    
    public int Level { get; set; }
    
    public string HashedPassword { get; set; }
    
}