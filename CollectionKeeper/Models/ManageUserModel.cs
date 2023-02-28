namespace CollectionKeeper.Models
{
    public class ManageUserModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool LockoutEnabled { get; set; }
    }
}
