namespace KickBlastStudentUI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordPlain { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
