using CollectionKeeper.Entities;

namespace CollectionKeeper.Models
{
    public class UserRoleModel
    {
        public CollectionUser User { get; set; }

        public bool IsAdmin { get; set; }
    }
}
