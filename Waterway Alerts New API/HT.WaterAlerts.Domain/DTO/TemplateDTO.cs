namespace HT.WaterAlerts.Domain
{
    public class TemplateDTO
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        /*public ApplicationUser MaptoTemplate(TemplateDTO templateDTO, Template template)
        {
            appUser.FirstName = user.FirstName;
            appUser.PhoneNumber = user.MobileNumber;
            appUser.LastName = user.LastName;
            appUser.SkipTutorial = user.SkipTutorial;
            appUser.SkipVideo = user.SkipVideo;

            if (user.Id.HasValue)
            {
                appUser.Id = user.Id.Value;
            }
            else
            {
                appUser.Email = user.Email;
            }
            return appUser;
        }*/
    }
}
