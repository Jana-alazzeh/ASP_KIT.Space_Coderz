using July_Team.Helpers;
using Microsoft.AspNetCore.Identity;

namespace July_Team.Services
{
    /// <summary>
    /// Service for managing user roles with enhanced permission and control level information.
    /// Provides detailed role information for user profiles without modifying existing role functionality.
    /// 
    /// Control Levels:
    /// - Admin: 100 (full system access)
    /// - Trainer: 70 (content management)
    /// - Member: 30 (basic access)
    /// - Guest: 0 (read-only)
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const string SOURCE = "UserRoleService";

        private static readonly Dictionary<string, RoleDefinition> _roleDefinitions = new()
        {
            ["Admin"] = new RoleDefinition
            {
                Description = "Full system administrator with complete access",
                ControlLevel = 100,
                BadgeColor = "danger",
                Permissions = new List<string>
                {
                    "Manage Users",
                    "Manage Roles",
                    "Manage Products",
                    "Manage Orders",
                    "Manage Courses",
                    "View Reports",
                    "System Configuration",
                    "Delete Content"
                }
            },
            ["super admin"] = new RoleDefinition
            {
                Description = "Super administrator with elevated privileges",
                ControlLevel = 100,
                BadgeColor = "dark",
                Permissions = new List<string>
                {
                    "Manage Users",
                    "Manage Roles",
                    "Manage Products",
                    "Manage Orders",
                    "Manage Courses",
                    "View Reports",
                    "System Configuration",
                    "Delete Content",
                    "Access Audit Logs"
                }
            },
            ["Trainer"] = new RoleDefinition
            {
                Description = "Course trainer with content management access",
                ControlLevel = 70,
                BadgeColor = "primary",
                Permissions = new List<string>
                {
                    "Manage Own Courses",
                    "View Enrolled Students",
                    "Create Course Content",
                    "Grade Assignments",
                    "View Basic Reports"
                }
            },
            ["Member"] = new RoleDefinition
            {
                Description = "Registered member with basic platform access",
                ControlLevel = 30,
                BadgeColor = "success",
                Permissions = new List<string>
                {
                    "View Products",
                    "Place Orders",
                    "View Own Orders",
                    "Enroll in Courses",
                    "Update Profile"
                }
            }
        };

        public UserRoleService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Gets comprehensive profile information for a user including all role details.
        /// This enhances the user profile view with permission visibility.
        /// </summary>
        public async Task<UserProfileInfo> GetUserProfileInfoAsync(IdentityUser user)
        {
            LoggingHelper.LogInfo(SOURCE, $"Getting profile info for user: {user.Email}");

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleDetails = new List<RoleDetailInfo>();

                foreach (var roleName in roles)
                {
                    roleDetails.Add(GetRoleDetailInfo(roleName));
                }

                var profileInfo = new UserProfileInfo
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    Roles = roles,
                    RoleDetails = roleDetails,
                    HighestControlLevel = roleDetails.Any() ? roleDetails.Max(r => r.ControlLevel) : 0,
                    MemberSince = null
                };

                LoggingHelper.LogSuccess(SOURCE, $"Profile info retrieved. Roles: {string.Join(", ", roles)}");
                return profileInfo;
            }
            catch (Exception ex)
            {
                LoggingHelper.LogError(SOURCE, $"Failed to get profile info for {user.Email}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets all defined roles with their complete details.
        /// Useful for admin panels showing role comparisons.
        /// </summary>
        public async Task<IEnumerable<RoleDetailInfo>> GetAllRolesWithDetailsAsync()
        {
            LoggingHelper.LogInfo(SOURCE, "Fetching all roles with details");

            var roles = _roleManager.Roles.ToList();
            var roleDetails = new List<RoleDetailInfo>();

            foreach (var role in roles)
            {
                if (role.Name != null)
                {
                    roleDetails.Add(GetRoleDetailInfo(role.Name));
                }
            }

            await Task.CompletedTask;
            return roleDetails;
        }

        /// <summary>
        /// Gets the control level for a role.
        /// Control level determines the scope of actions a user can perform.
        /// </summary>
        public int GetRoleControlLevel(string roleName)
        {
            if (_roleDefinitions.TryGetValue(roleName, out var definition))
            {
                return definition.ControlLevel;
            }
            return 0;
        }

        /// <summary>
        /// Gets the permissions associated with a role.
        /// </summary>
        public IEnumerable<string> GetRolePermissions(string roleName)
        {
            if (_roleDefinitions.TryGetValue(roleName, out var definition))
            {
                return definition.Permissions;
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Builds a RoleDetailInfo object for a given role name.
        /// Falls back to default values for undefined roles.
        /// </summary>
        private RoleDetailInfo GetRoleDetailInfo(string roleName)
        {
            if (_roleDefinitions.TryGetValue(roleName, out var definition))
            {
                return new RoleDetailInfo
                {
                    RoleName = roleName,
                    Description = definition.Description,
                    ControlLevel = definition.ControlLevel,
                    Permissions = definition.Permissions,
                    BadgeColor = definition.BadgeColor
                };
            }

            return new RoleDetailInfo
            {
                RoleName = roleName,
                Description = "Custom role",
                ControlLevel = 10,
                Permissions = new List<string> { "Basic Access" },
                BadgeColor = "secondary"
            };
        }

        /// <summary>
        /// Internal class for role definition storage.
        /// </summary>
        private class RoleDefinition
        {
            public string Description { get; set; } = string.Empty;
            public int ControlLevel { get; set; }
            public string BadgeColor { get; set; } = "secondary";
            public List<string> Permissions { get; set; } = new List<string>();
        }
    }
}
