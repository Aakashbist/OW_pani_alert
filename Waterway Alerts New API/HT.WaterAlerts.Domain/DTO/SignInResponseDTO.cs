namespace HT.WaterAlerts.Domain
{
    public class SignInResponseDTO
    {
        public string Email { get; set; }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExpireIn { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public bool SkipVideo { get; set; }
        public bool SkipTutorial { get; set; }
        public bool SkipTermsAndConditions { get; set; }
        public List<string> Roles { get; set; }
    }
}
