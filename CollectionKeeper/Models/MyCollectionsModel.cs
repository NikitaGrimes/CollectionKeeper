using CollectionKeeper.Entities;

namespace CollectionKeeper.Models
{
    public class MyCollectionsModel
    {
        public string OwnerName { get; set; }

        public List<Collection> Collections { get; set; }
    }
}
