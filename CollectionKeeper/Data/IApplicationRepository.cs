using CollectionKeeper.Entities;

namespace CollectionKeeper.Data
{
    public interface IApplicationRepository
    {
        public IEnumerable<CollectionUser> GetAllUsers();

        public CollectionUser GetUserById(string id);

        public CollectionUser GetUserByName(string name);

        public bool IsUserExist(string name);

        public bool RemoveUser(CollectionUser user);

        public bool BlockUserById(string id);

        public bool UnBlockUserById(string id);

        public bool GetBlockUserStatus(string userName);

        public IEnumerable<Collection> GetAllCollections();

        public IEnumerable<Collection> GetCollectionByUserName(string userName);

        public Collection GetCollectionById(int id);

        public bool AddCollection(Collection collection);

        public bool EditCollection(Collection collection);

        public bool DeleteCollectionById(int id);

        public IEnumerable<Collection> GetBiggest3Collections();

        public IEnumerable<Collection> GetFiltersSortedCollectionsByUserName(string filter, string sort, string userName);

        public IEnumerable<Collection> GetFiltersSortedCollections(string filter, string sort);

        public bool AddItemCollection(CollectionItem item);

        public bool EditItemCollection(CollectionItem item);

        public IEnumerable<CollectionItem> GetLast5Items();

        public IEnumerable<CollectionItem> GetAllItems();

        public IEnumerable<CollectionItem> GetTermsItems(string term);

        public IEnumerable<CollectionItem> GetSortFilterItems(int id, string fieldSort, string filter);

        public bool DeleteItemCollectionById(int id);

        public CollectionItem GetItemById(int id);

        public IEnumerable<CollectionItem> GetTagItemsById(int[] tagsId);

        public IEnumerable<Topic> GetAllTopics();

        public IEnumerable<Tag> GetAllTags();

        public IEnumerable<string> GetAllTagsNames();

        public bool AddTag(Tag tag);

        public bool AddComment(Comment comment);

        public bool DeleteCommentById(int id);

        public Comment GetCommentById(int id);
    }
}
