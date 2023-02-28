using CollectionKeeper.Entities;
using KnowledgePicker.WordCloud;
using KnowledgePicker.WordCloud.Primitives;

namespace CollectionKeeper.Models
{
    public class HomeModel
    {
        public List<CollectionItem> LastItems { get; set; }

        public List<Collection> Collections { get; set; }

        public List<TagModel> Tags { get; set; }
    }
}
