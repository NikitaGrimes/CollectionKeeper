using Microsoft.AspNetCore.Identity;

namespace CollectionKeeper.Entities
{
    public class CollectionUser : IdentityUser
    {
        public ICollection<Collection> Collections { get; set; }

        public ICollection<CollectionItem> LikedCollections { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
