namespace DevIO.Api.ViewModels.Auth
{
    public class LoginResponseViewModel
    {
        public string AccessToken { get; set; }
        public double ExpireIn { get; set; }
        public UserTokenViewModel UserToken { get; set; }
    }
}
