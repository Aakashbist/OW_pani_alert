namespace HT.Overwatch.API.DTO
{
    public class UserPolicyAuthorisationListDto
    {
        public IList<UserPolicyAuthorisationDto> PolicyAuthorisations { get; }

        public UserPolicyAuthorisationListDto() : this(new List<UserPolicyAuthorisationDto>())
        { }

        public UserPolicyAuthorisationListDto(IList<UserPolicyAuthorisationDto> policyAuthorisations)
        {
            PolicyAuthorisations = policyAuthorisations;
        }

    }
}
