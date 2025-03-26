namespace CustomerService.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string userName);
    }
}
