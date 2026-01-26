using July_Team.Models;
using Microsoft.AspNetCore.Identity;

namespace July_Team.Services
{
    /// <summary>
    /// Interface for user and role management operations.
    /// Provides enhanced role information including permissions and control levels.
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>
        /// Gets detailed role information for a user, including permissions.
        /// </summary>
        /// <param name="user">The user to get roles for</param>
        Task<UserProfileInfo> GetUserProfileInfoAsync(IdentityUser user);

        /// <summary>
        /// Gets all roles with their permission details.
        /// </summary>
        Task<IEnumerable<RoleDetailInfo>> GetAllRolesWithDetailsAsync();

        /// <summary>
        /// Gets the control level (0-100) for a specific role.
        /// Higher values indicate more permissions.
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        int GetRoleControlLevel(string roleName);

        /// <summary>
        /// Gets the list of permissions for a specific role.
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        IEnumerable<string> GetRolePermissions(string roleName);
    }

    /// <summary>
    /// Model containing detailed user profile information including role details.
    /// Used for displaying enhanced profile information.
    /// </summary>
    public class UserProfileInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public List<RoleDetailInfo> RoleDetails { get; set; } = new List<RoleDetailInfo>();
        public int HighestControlLevel { get; set; }
        public DateTime? MemberSince { get; set; }
    }

    /// <summary>
    /// Model containing detailed information about a role.
    /// Includes permissions and control level for display.
    /// </summary>
    public class RoleDetailInfo
    {
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ControlLevel { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
        public string BadgeColor { get; set; } = "secondary";
    }
}
