namespace AerionDyseti.API.Auth.Models.ResponseDTOs
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public int Expiration { get; set; }
    }
}
