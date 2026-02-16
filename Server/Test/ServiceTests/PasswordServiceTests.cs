using DevOpsAppService.Services;

namespace Test.ServiceTests;

public class PasswordServiceTests(PasswordService passwordService)
{
    [Fact]
    public void HashPassword_ThenVerify_ReturnsTrueForCorrectPassword()
    {
        var plainPassword = "CorrectHorseBatteryStaple!";

        var hash = passwordService.HashPassword(plainPassword);

        Assert.False(string.IsNullOrWhiteSpace(hash));
        Assert.NotEqual(plainPassword, hash);
        Assert.True(passwordService.VerifyPassword(plainPassword, hash));
        Assert.False(passwordService.VerifyPassword("wrong-password", hash));
    }
}
