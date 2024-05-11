using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Model
{
    public class UserModel
    {
        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? VerificationToken { get; set; }

        //chua biet nen them AccessToken vao khong
        //public string? AccessToken { get; set; }
        public int? RoleId { get; set; }
    }
}
