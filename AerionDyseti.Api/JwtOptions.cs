namespace AerionDyseti
{

    /// <summary>
    /// Class to store JWT Configuration options for Injection into other systems.
    /// </summary>
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
    }
}
