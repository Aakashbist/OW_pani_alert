using Microsoft.AspNetCore.Identity;
using HT.WaterAlerts.Domain;
using Microsoft.AspNetCore.JsonPatch;

namespace HT.WaterAlerts.Service
{
    public interface IUserService
    {
        DataTableResponseDTO GetUsers(DataTableRequestDTO request);
        IEnumerable<UsersDTO> GetUsers();
        UsersDTO GetUser(Guid id);
        Task<IdentityResult> CreateUser(UsersDTO userDTO);
        Task<IdentityResult> UpdateUser(UsersDTO userDTO);
        Task<IdentityResult> DeleteUser(Guid id);
        Task<IdentityResult> UpdateUserPartial(Guid id, JsonPatchDocument patchUser, bool isAdmin);
    }

}
