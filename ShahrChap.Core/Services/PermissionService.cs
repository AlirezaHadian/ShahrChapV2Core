using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Context;
using ShahrChap.DataLayer.Entities.Permissions;
using ShahrChap.DataLayer.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShahrChap.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private ShahrChapContext _context;
        private IUserService _userService;
        public PermissionService(ShahrChapContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public void AddPermissionsToRole(int roleId, List<int> permissions)
        {
           foreach(var item in permissions)
            {
                _context.RolePermission.Add(new RolePermission()
                {
                    RoleId = roleId,
                    PermissionId = item
                });
            }
            _context.SaveChanges();
        }

        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach (var roleId in roleIds)
            {
                _context.UserRoles.Add(new UserRole()
                {
                    UserId = userId,
                    RoleId = roleId
                });
            }
            _context.SaveChanges();
        }

        public bool CheckPermission(int permissionId, string username)
        {
            int userId = _userService.GetUserIdWithUserName(username);

            List<int> userRoles = GetUserRoles(userId);
        
            if(!userRoles.Any())
                return false;

            List<int> rolesPermission = GetRolesPermission(permissionId);

            return rolesPermission.Any(p => userRoles.Contains(p));
        }

        public void DeleteRole(Role role)
        {
            role.IsDelete = true;
            _context.Roles.Update(role);
            _context.SaveChanges();
        }

        public void EditRolesUser(int userId, List<int> rolesId)
        {
            //Delete all user roles
            _context.UserRoles.Where(r => r.UserId == userId).ToList().ForEach(r => _context.UserRoles.Remove(r));
            //add new roles
            AddRolesToUser(rolesId, userId);
        }

        public List<Permission> GetPermissions()
        {
           return _context.Permission.ToList();
        }

        public Role GetRoleById(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.ToList();
        }

        public List<int> GetUserRoles(int userId)
        {
            List<int> UserRoles = _context.UserRoles
                .Where(r => r.UserId == userId)
                .Select(r => r.RoleId).ToList();
            return UserRoles;
        }

        public List<int> PermissionsRole(int roleId)
        {
            return _context.RolePermission
                .Where(p=> p.RoleId == roleId)
                .Select(r=> r.PermissionId).ToList();
        }

        public List<int> GetRolesPermission(int permissionId)
        {
            List<int> RolesPermission = _context.RolePermission
                .Where(r => r.PermissionId == permissionId)
                .Select(r => r.RoleId).ToList();
            return RolesPermission;
        }

        public void UpdatePermissionsRole(int roleId, List<int> permissions)
        {
            _context.RolePermission
                .Where(p => p.RoleId == roleId).ToList()
                .ForEach(p => _context.RolePermission.Remove(p));
            AddPermissionsToRole(roleId, permissions);
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }
    }
}
