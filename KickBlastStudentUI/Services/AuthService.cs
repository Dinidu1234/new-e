using KickBlastStudentUI.Data;
using Microsoft.EntityFrameworkCore;

namespace KickBlastStudentUI.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    public bool IsLoggedIn { get; private set; }

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username && x.PasswordPlain == password);
        IsLoggedIn = user != null;
        return IsLoggedIn;
    }

    public void Logout() => IsLoggedIn = false;
}
