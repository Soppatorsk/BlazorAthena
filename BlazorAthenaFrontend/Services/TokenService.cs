namespace BlazorAthenaFrontend.Services
{
    public class TokenService
    {
        private string _token;

        public string Token
        {
            get => _token;
            set => _token = value;
        }
    }
}
