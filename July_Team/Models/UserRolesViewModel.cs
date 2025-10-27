namespace July_Team.Models
{
    public class UserRolesViewModel
    {
        public  string UserId { get; set; }
        public string Email { get; set; }
        public List<RolesViewModel> Roles { get; set; }=new List<RolesViewModel>();
    }
}
