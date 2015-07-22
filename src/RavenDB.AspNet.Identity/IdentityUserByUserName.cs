namespace RavenDB.AspNet.Identity
{
    public sealed class IdentityUserByUserName
    {
        public string UserId { get; set; }
        public string UserName { get; set; }

        public IdentityUserByUserName(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}