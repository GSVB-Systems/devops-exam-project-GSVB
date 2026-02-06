namespace DevOpsAppService.Services;

public class PasswordService
{
    public string HashPassword(string plainPassword)
    {

        return BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);
    }


    public bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
    }

}