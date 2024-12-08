namespace AXProductApp.Interfaces
{
    public interface IManageFireBaseTokenService
    {
        public Task CreateToken();

        public Task SendTokenToServer(string token);
    }
}
