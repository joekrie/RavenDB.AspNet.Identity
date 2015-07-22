namespace RavenDB.AspNet.Identity
{
    public class IdentityRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; internal set; }
    }
}
