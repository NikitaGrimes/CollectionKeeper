using CollectionKeeper.Entities;

namespace CollectionKeeper.Models
{
    public class IndexItemModel
    {
        public List<CollectionItem> CollectionItems { get; set; }

        public Collection Collection { get; set; }
    }
}
