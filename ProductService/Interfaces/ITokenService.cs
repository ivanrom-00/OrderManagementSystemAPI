namespace ProductService.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string userName);
    }
}
