using CollectionKeeper.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeeper.Data
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationContext _context;

        public ApplicationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IEnumerable<CollectionUser> GetAllUsers()
        {
            return _context.Users;
        }

        public CollectionUser GetUserById(string id)
        {
            if (id == null) return null;

            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public CollectionUser GetUserByName(string name)
        {
            if (name == null) return null;

            return _context.Users.FirstOrDefault(u => u.UserName == name);
        }

        public bool RemoveUser(CollectionUser user)
        {
            if(user == null) return false;

            _context.Users.Remove(user);
            return SaveAll();
        }

        public bool BlockUserById(string id)
        {
            if(id == null) return false;

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.LockoutEnabled = false;
                return SaveAll();
            }

            return false;
        }

        public bool UnBlockUserById(string id)
        {
            if (id == null) return false;

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.LockoutEnabled = true;
                return SaveAll();
            }

            return false;
        }

        public bool GetBlockUserStatus(string userName)
        {
            if (userName == null) return true;

            return !_context.Users.FirstOrDefault(u => u.UserName == userName).LockoutEnabled;
        }

        public IEnumerable<Collection> GetAllCollections()
        {
            return _context.Collections.Include(c => c.Owner)
                .Include(c => c.Topic);
        }

        public IEnumerable<Collection> GetCollectionByUserName(string userName)
        {
            if (userName == null) return Enumerable.Empty<Collection>(); ;

            return _context.Collections.Where(col => col.Owner.UserName == userName)
                .Include(c => c.Topic);
        }

        public Collection GetCollectionById(int id)
        {
            return _context.Collections
                .Include(c => c.Topic)
                .Include(c => c.Owner)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Tags)
                .FirstOrDefault(col => col.Id == id);
        }

        public bool AddCollection(Collection collection)
        {
            if (collection == null) return false;

            _context.Collections.Add(collection);
            return SaveAll();
        }

        public bool EditCollection(Collection collection)
        {
            if(collection == null) return false;

            _context.Collections.Update(collection);
            return SaveAll();
        }

        public bool DeleteCollectionById(int id)
        {
            Collection collection = GetCollectionById(id);
            if(collection == null) return false;

            _context.Collections.Remove(collection);
            return SaveAll();
        }

        public IEnumerable<Collection> GetBiggest3Collections()
        {
            return _context.Collections
                .Include(c => c.Topic)
                .OrderByDescending(c => c.Items.Count)
                .Take(3);
        }

        public IEnumerable<Collection> GetFiltersSortedCollectionsByUserName(string filter, string sort, string userName)
        {
            filter = filter != null ? filter : String.Empty;
            sort = sort != null ? sort : String.Empty;
            if (userName == null) return Enumerable.Empty<Collection>(); ;

            IEnumerable<Collection> collections = _context.Collections.Where(c => c.Owner.UserName == userName)
                .Where(
                c => c.Name.Contains(filter) ||
                c.Description.Contains(filter) ||
                c.Topic.Name.Contains(filter))
                .Include(c => c.Topic);
            if (sort == "Name")
                return collections.OrderBy(c => c.Name);
            else if (sort == "Des")
                return collections.OrderBy(c => c.Description);
            else if (sort == "Top")
                return collections.OrderBy(c => c.Topic.Name);
            else return collections;
        }

        public IEnumerable<Collection> GetFiltersSortedCollections(string filter, string sort)
        {
            filter = filter != null ? filter : String.Empty;
            sort = sort != null ? sort : String.Empty;
            IEnumerable<Collection> collections = _context.Collections
                .Where(
                c => c.Name.Contains(filter) ||
                c.Description.Contains(filter) ||
                c.Topic.Name.Contains(filter))
                .Include(c => c.Topic);
            if (sort == "Name")
                return collections.OrderBy(c => c.Name);
            else if (sort == "Des")
                return collections.OrderBy(c => c.Description);
            else if (sort == "Top")
                return collections.OrderBy(c => c.Topic.Name);
            else return collections;
        }

        public bool AddItemCollection(CollectionItem item)
        {
            if(item == null) return false;

            _context.Items.Add(item);
            return SaveAll();
        }

        public IEnumerable<CollectionItem> GetLast5Items()
        {
            return _context.Items.OrderByDescending(i => i.CreatedDate).Take(5);
        }

        public IEnumerable<CollectionItem> GetAllItems()
        {
            return _context.Items
                .Include(i => i.Tags);
        }

        public IEnumerable<CollectionItem> GetTermsItems(string term)
        {
            if (term == null) return Enumerable.Empty<CollectionItem>();

            return _context.Items
                .Where(i => i.Name.Contains(term) ||
                i.NumberValueField_1.ToString().Contains(term) ||
                i.NumberValueField_2.ToString().Contains(term) ||
                i.NumberValueField_3.ToString().Contains(term) ||
                i.StringValueField_1.Contains(term) ||
                i.StringValueField_2.Contains(term) ||
                i.StringValueField_3.Contains(term) ||
                i.TextValueField_1.Contains(term) ||
                i.TextValueField_2.Contains(term) ||
                i.TextValueField_3.Contains(term) ||
                i.DateValueField_1.ToString().Contains(term) ||
                i.DateValueField_2.ToString().Contains(term) ||
                i.DateValueField_3.ToString().Contains(term) ||
                i.Collection.Description.Contains(term) ||
                i.Comments.Any(c => c.Text.Contains(term)));
        }

        public IEnumerable<CollectionItem> GetSortFilterItems(int id, string fieldSort, string filter)
        {
            IEnumerable<CollectionItem> items;
            if (filter != null && filter != String.Empty)
                items = _context.Items.Where(i => i.Collection.Id == id)
                .Where(i => i.Name.Contains(filter) ||
                i.NumberValueField_1.ToString().Contains(filter) ||
                i.NumberValueField_2.ToString().Contains(filter) ||
                i.NumberValueField_3.ToString().Contains(filter) ||
                i.StringValueField_1.Contains(filter) ||
                i.StringValueField_2.Contains(filter) ||
                i.StringValueField_3.Contains(filter) ||
                i.TextValueField_1.Contains(filter) ||
                i.TextValueField_2.Contains(filter) ||
                i.TextValueField_3.Contains(filter) ||
                i.DateValueField_1.ToString().Contains(filter) ||
                i.DateValueField_2.ToString().Contains(filter) ||
                i.DateValueField_3.ToString().Contains(filter));
            else
                items = _context.Items.Where(i => i.Collection.Id == id);
            if (fieldSort == null) return items;
            switch(fieldSort)
            {
                case "name↓":
                    items = items.OrderBy(i => i.Name);
                    break;
                case "name↑":
                    items = items.OrderByDescending(i => i.Name);
                    break;
                case "NumberNameField_1↓":
                    items = items.OrderBy(i => i.NumberValueField_1);
                    break;
                case "NumberNameField_1↑":
                    items = items.OrderByDescending(i => i.NumberValueField_1);
                    break;
                case "NumberNameField_2↓":
                    items = items.OrderBy(i => i.NumberValueField_2);
                    break;
                case "NumberNameField_2↑":
                    items = items.OrderByDescending(i => i.NumberValueField_2);
                    break;
                case "NumberNameField_3↓":
                    items = items.OrderBy(i => i.NumberValueField_3);
                    break;
                case "NumberNameField_3↑":
                    items = items.OrderByDescending(i => i.NumberValueField_3);
                    break;
                case "StringNameField_1↓":
                    items = items.OrderBy(i => i.StringValueField_1);
                    break;
                case "StringNameField_1↑":
                    items = items.OrderByDescending(i => i.StringValueField_1);
                    break;
                case "StringNameField_2↓":
                    items = items.OrderBy(i => i.StringValueField_2);
                    break;
                case "StringNameField_2↑":
                    items = items.OrderByDescending(i => i.StringValueField_2);
                    break;
                case "StringNameField_3↓":
                    items = items.OrderBy(i => i.StringValueField_3);
                    break;
                case "StringNameField_3↑":
                    items = items.OrderByDescending(i => i.StringValueField_3);
                    break;
                case "TextNameField_1↓":
                    items = items.OrderBy(i => i.TextValueField_1);
                    break;
                case "TextNameField_1↑":
                    items = items.OrderByDescending(i => i.TextValueField_1);
                    break;
                case "TextNameField_2↓":
                    items = items.OrderBy(i => i.TextValueField_2);
                    break;
                case "TextNameField_2↑":
                    items = items.OrderByDescending(i => i.TextValueField_2);
                    break;
                case "TextNameField_3↓":
                    items = items.OrderBy(i => i.TextValueField_3);
                    break;
                case "TextNameField_3↑":
                    items = items.OrderByDescending(i => i.TextValueField_3);
                    break;
                case "DateNameField_1↓":
                    items = items.OrderBy(i => i.DateValueField_1);
                    break;
                case "DateNameField_1↑":
                    items = items.OrderByDescending(i => i.DateValueField_1);
                    break;
                case "DateNameField_2↓":
                    items = items.OrderBy(i => i.DateValueField_2);
                    break;
                case "DateNameField_2↑":
                    items = items.OrderByDescending(i => i.DateValueField_2);
                    break;
                case "DateNameField_3↓":
                    items = items.OrderBy(i => i.DateValueField_3);
                    break;
                case "DateNameField_3↑":
                    items = items.OrderByDescending(i => i.DateValueField_3);
                    break;
                case "BoolNameField_1↓":
                    items = items.OrderBy(i => i.BoolValueField_1);
                    break;
                case "BoolNameField_1↑":
                    items = items.OrderByDescending(i => i.BoolValueField_1);
                    break;
                case "BoolNameField_2↓":
                    items = items.OrderBy(i => i.BoolValueField_2);
                    break;
                case "BoolNameField_2↑":
                    items = items.OrderByDescending(i => i.BoolValueField_2);
                    break;
                case "BoolNameField_3↓":
                    items = items.OrderBy(i => i.BoolValueField_3);
                    break;
                case "BoolNameField_3↑":
                    items = items.OrderByDescending(i => i.BoolValueField_3);
                    break;
            }
            return items;
        }

        public bool EditItemCollection(CollectionItem item)
        {
            if(item == null) return false;

            _context.Items.Update(item);
            return SaveAll();
        }

        public bool DeleteItemCollectionById(int id)
        {
            CollectionItem item = GetItemById(id);
            if(item == null) return false;

            _context.Items.Remove(item);
            return SaveAll();
        }

        public CollectionItem GetItemById(int id)
        {
            return _context.Items
                .Include(i => i.Collection)
                    .ThenInclude(c => c.Owner)
                .Include(i => i.Tags)
                .Include(i => i.LikedUsers)
                .Include(i => i.Comments.OrderByDescending(c => c.CreatedTime))
                    .ThenInclude(c => c.Author)
                .FirstOrDefault(i => i.Id == id) ;
        }

        public IEnumerable<CollectionItem> GetTagItemsById(int[] tagsId)
        {
            if(tagsId == null) return Enumerable.Empty<CollectionItem>();
            return _context.Items.Include(i => i.Tags).Where(i => i.Tags.Any(t => tagsId.Any(id => t.Id == id)));
        }

        public IEnumerable<Topic> GetAllTopics()
        {
            return _context.Topics;
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return _context.Tags
                .Include(t => t.Items);
        }

        public IEnumerable<string> GetAllTagsNames()
        {
            return _context.Tags.Select(x => x.Name);
        }

        public bool AddTag(Tag tag)
        {
            if (tag == null) return false;

            _context.Tags.Add(tag);
            return SaveAll();
        }

        public bool AddComment(Comment comment)
        {
            if (comment == null) return false;

            _context.Comments.Add(comment);
            return SaveAll();
        }

        public Comment GetCommentById(int id)
        {
            return _context.Comments
                .Include(c => c.Author)
                .Include(c => c.CollectionItem)
                    .ThenInclude(i => i.Collection)
                        .ThenInclude(c => c.Owner)
                .FirstOrDefault(c => c.Id == id);
        }

        public bool DeleteCommentById(int id)
        {
            Comment comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            if(comment == null) return false;

            _context.Comments.Remove(comment);
            return SaveAll();
        }

        private bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
