using ShahrChap.DataLayer.Entities.Permissions;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles
        List<Role> GetRoles();
        int AddRole(Role role);
        Role GetRoleById(int roleId);
        void UpdateRole(Role role);
        void DeleteRole(Role role);
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditRolesUser(int userId, List<int> rolesId);

        #endregion
        #region Permission
        List<Permission> GetPermissions();
        void AddPermissionsToRole(int roleId, List<int> permissions);
        List<int> PermissionsRole(int roleId);
        void UpdatePermissionsRole(int roleId, List<int> permissions);
        List<int> GetUserRoles(int userId);
        List<int> GetRolesPermission(int permissionId);
        bool CheckPermission(int permissionId, string username);
        #endregion
    }
}
