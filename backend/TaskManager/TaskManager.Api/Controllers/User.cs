using TaskManager.Api.Data.Entities;

internal class User : Users
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public object VerificationToken { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ModificationDate { get; set; }
    public object VerificationDate { get; set; }
}