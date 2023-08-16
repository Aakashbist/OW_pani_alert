using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using System.Linq.Dynamic.Core;
using System.Transactions;
using Microsoft.AspNetCore.JsonPatch;

namespace HT.WaterAlerts.Service
{
    public class UserService: IUserService
    {

        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UserService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public DataTableResponseDTO GetUsers(DataTableRequestDTO request)
        {
            var param = ServiceHelper.GetDataTableParams(request);
            var criteria = ServiceHelper.GetFilterPredicate<ApplicationUser>(param.Filter);
            var orderBy = ServiceHelper.GetOrderPredicate<ApplicationUser>(param.OrderColumn);
            var items = _uow.GetRepository<ApplicationUser>().Get(criteria, orderBy, param.OrderDirection, request.PageNumber, request.PageSize);
            items = items.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);
            var users = items.Select(x => new UsersDTO().MapToUserDTO(x));
            int totalPageCount = _uow.GetRepository<ApplicationUser>().Count(criteria);
            int totalPage = (int)Math.Ceiling((double)totalPageCount / request.PageSize);
            return new DataTableResponseDTO
            {
                TotalCount = totalPageCount,
                TotalPage = totalPage,
                CurrentPage = request.PageNumber,
                Data = users.ToArray()
            };
        }

        public IEnumerable<UsersDTO> GetUsers()
        {
            var items = _uow.GetRepository<ApplicationUser>().GetAll();
            return items.Select(x => new UsersDTO().MapToUserDTO(x));
        }

        public UsersDTO GetUser(Guid id)
        {
            var item = _uow.GetRepository<ApplicationUser>().Get(u => u.Id.Equals(id));
            ApplicationUser user = item.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).SingleOrDefault();
            return user != null ? new UsersDTO().MapToUserDTO(user) : null;
        }

        public async Task<IdentityResult> CreateUser(UsersDTO user)
        {
            ApplicationUser entity = new ApplicationUser();
            entity = user.MaptoApplicationUser(user, entity);
            entity.CreatedDate = DateTime.Now;
            if (user.Roles != null && user.Roles.Count > 0)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        IdentityResult result = await _userManager.CreateAsync(entity, user.Password);
                        if (result.Succeeded)
                        {
                            result = await _userManager.AddToRolesAsync(entity, user.Roles);
                        }
                        scope.Complete();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        return IdentityResult.Failed(new IdentityError() { Code = "CreateUserError", Description = ex.Message });
                    }
                }
            }
            return IdentityResult.Failed(new IdentityError() { Code = "InvalidRole", Description = "Role of the user is invalid." });
        }

        public async Task<IdentityResult> UpdateUser(UsersDTO user)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var userEntity = _uow.GetRepository<ApplicationUser>().GetById(user.Id);
                    userEntity = user.MaptoApplicationUser(user, userEntity);
                    userEntity.ModifiedDate = DateTime.Now;
                    IdentityResult result = await _userManager.UpdateAsync(userEntity);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            var passwordValidator = IsPasswordValid(user.Password);
                            if (passwordValidator.Result.Succeeded)
                            {
                                var token = await _userManager.GeneratePasswordResetTokenAsync(userEntity);
                                result = await _userManager.ResetPasswordAsync(userEntity, token, user.Password);
                            }
                            else
                            {
                                scope.Dispose();
                                return passwordValidator.Result;
                            }
                        }
                        if (user.Roles != null && user.Roles.Count > 0)
                        {
                            var roles = await _userManager.GetRolesAsync(userEntity);
                            if (roles.Count > 0)
                            {
                                await _userManager.RemoveFromRolesAsync(userEntity, roles);
                            }
                            result = await _userManager.AddToRolesAsync(userEntity, user.Roles);
                        }
                    }
                    scope.Complete();
                    return result;
                }
                catch(Exception ex)
                {
                    scope.Dispose();
                    return IdentityResult.Failed(new IdentityError() { Code = "UpdateUserError", Description = ex.Message });
                }
            }
        }

        public async Task<IdentityResult> UpdateUserPartial(Guid id, JsonPatchDocument patchUser, bool isAdmin)
        {
            var pathList = patchUser.Operations.Select(x => x.path.ToLower());
            List<string> allowedColumns = new List<string> { "firstname", "lastname", "password", "skipvideo", "skiptutorial", "skiptermsandconditions", "phonenumber" };
            
            if(pathList.Except(allowedColumns).Count() > 0 && !isAdmin)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "PatchUserError", Description = "Invalid Parameters" });
            }
            else
            {
                ApplicationUser user = _uow.GetRepository<ApplicationUser>().GetById(id);
                user.ModifiedDate = DateTime.Now;
                var operationList = patchUser.Operations.Where(o => o.path.Equals("Password") || o.path.Equals("password"));
                if (operationList.Count() == 1)
                {
                    var passwordValidator = IsPasswordValid(operationList.First().value.ToString());
                    if (passwordValidator.Result.Succeeded)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        return await _userManager.ResetPasswordAsync(user, token, operationList.First().value.ToString());
                    }
                    else
                    {
                        return passwordValidator.Result;
                    }
                }
                else
                {
                    patchUser.ApplyTo(user);
                    return await _userManager.UpdateAsync(user);
                }
            }
        }

        public async Task<IdentityResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user != null)
            {
                return await _userManager.DeleteAsync(user);
            }
            return IdentityResult.Failed(new IdentityError() { Code = "DeleteUserError", Description = "User is not found." });
        }

        public async Task<IdentityResult> IsPasswordValid(string password)
        {
            var validators = _userManager.PasswordValidators;
            var result = new IdentityResult();
            foreach (var validator in validators)
            {
                result = await validator.ValidateAsync(_userManager, null, password);
            }
            return result;
        }
    } 
}
