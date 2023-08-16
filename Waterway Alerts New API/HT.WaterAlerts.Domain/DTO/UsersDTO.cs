namespace HT.WaterAlerts.Domain
{
    public class UsersDTO
    {
        public Guid? Id { get; set; }
        public string FirstName { get; set; }
        public string MobileNumber { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }
        public bool SkipVideo { get; set; }
        public bool SkipTutorial { get; set; }
        public List<string> Roles { get; set; }

        public UsersDTO MapToUserDTO(ApplicationUser user)
        {
            return new UsersDTO()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                MobileNumber = user.PhoneNumber,
                LastName = user.LastName,
                Status = user.Status,
                Type = user.Type,
                SkipTutorial = user.SkipTutorial,
                SkipVideo = user.SkipVideo,
                Roles = user.UserRoles != null ? user.UserRoles.Select(q => q.Role.Name).ToList() : null
            };
        }

        public ApplicationUser MaptoApplicationUser(UsersDTO user, ApplicationUser appUser)
        {
            appUser.FirstName = user.FirstName;
            appUser.PhoneNumber = user.MobileNumber;
            appUser.Status = user.Status;
            appUser.LastName = user.LastName;
            appUser.SkipTutorial = user.SkipTutorial;
            appUser.SkipVideo = user.SkipVideo;
            appUser.Type = user.Type;

            if (user.Id.HasValue)
            {
                appUser.Id = user.Id.Value;
            }
            else
            {
                appUser.Email = user.Email;
            }
            return appUser;
        }
    }
}
