using CollectionKeeper.Data;
using CollectionKeeper.Entities;
using CollectionKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CollectionKeeper.Controllers
{
    public class ItemController : Controller
    {

        private readonly SignInManager<CollectionUser> _signInManager;

        private readonly UserManager<CollectionUser> _userManager;

        private readonly IApplicationRepository _applicationRepository;

        private readonly IStringLocalizer<ItemController> _localizer;

        public ItemController(SignInManager<CollectionUser> signInManager, UserManager<CollectionUser> userManager,
            IApplicationRepository applicationRepository, IStringLocalizer<ItemController> localizer)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _applicationRepository = applicationRepository;
            _localizer = localizer;
        }

        public IActionResult Index(int? id)
        {
            LogoutAfterBlocked();

            if (id != null)
            {
                Collection collection = _applicationRepository.GetCollectionById((int)id);
                if(collection == null)
                    return RedirectToAction("MyCollection", "Collection");

                IndexItemModel indexItemModel = new();
                indexItemModel.Collection = collection;
                indexItemModel.CollectionItems = collection.Items.ToList();
                return View(indexItemModel);
            }

            return RedirectToAction("MyCollection", "Collection");
        }

        [HttpGet]
        public IActionResult SortFilterItems(int id, string fieldNameSort, string filter)
        {
            List<CollectionItem> items = _applicationRepository.GetSortFilterItems(id, fieldNameSort, filter).ToList();
            Collection collection = _applicationRepository.GetCollectionById(id);
            if (collection == null)
                collection = new Collection();

            IndexItemModel indexItemModel = new();
            indexItemModel.Collection = collection;
            indexItemModel.CollectionItems = items;
            return PartialView("_IndexItem", indexItemModel);
        }

        public IActionResult Detail(int? id)
        {
            LogoutAfterBlocked();

            if (id != null)
            {
                CollectionItem item = _applicationRepository.GetItemById((int)id);
                if (item == null)
                    return RedirectToAction("Index", "Collection");

                return View(item);
            }

            return RedirectToAction("Index", "Collection"); 
        }

        public IActionResult Edit(int? id)
        {
            if (LogoutAfterBlocked()) 
                RedirectToAction("Detail", "Item", new {id = id});

            if (id != null)
            {
                CollectionItem item = _applicationRepository.GetItemById((int)id);
                if(item == null)
                    return RedirectToAction("Index", "Collection");

                if (User.Identity.Name != item.Collection.Owner.UserName && !User.IsInRole("admin"))
                    return RedirectToAction("Index", "Collection");

                ItemEditModel model = new ItemEditModel();
                model.AutocompledTags = _applicationRepository.GetAllTags().Select(t => t.Name).ToList();
                model.Id = item.Id;
                model.Name = item.Name;
                List<string> tags = new List<string>();
                foreach(var tag in item.Tags)
                {
                    tags.Add(tag.Name);
                }

                model.Tags = tags;
                model.NumberValueField_1 = item.NumberValueField_1;
                model.NumberValueField_2 = item.NumberValueField_2;
                model.NumberValueField_3 = item.NumberValueField_3;
                model.StringValueField_1 = item.StringValueField_1;
                model.StringValueField_2 = item.StringValueField_2;
                model.StringValueField_3 = item.StringValueField_3;
                model.TextValueField_1 = item.TextValueField_1;
                model.TextValueField_2 = item.TextValueField_2;
                model.TextValueField_3 = item.TextValueField_3;
                if (item.DateValueField_1 != null)
                {
                    model.DateValueField_1 = (DateTime)item.DateValueField_1;
                    model.TimeValueField_1 = (DateTime)item.DateValueField_1;
                }

                if (item.DateValueField_2 != null)
                {
                    model.DateValueField_2 = (DateTime)item.DateValueField_2;
                    model.TimeValueField_2 = (DateTime)item.DateValueField_2;
                }

                if (item.DateValueField_3 != null)
                {
                    model.DateValueField_3 = (DateTime)item.DateValueField_3;
                    model.TimeValueField_3 = (DateTime)item.DateValueField_3;
                }

                model.BoolValueField_1 = item.BoolValueField_1 == null ? false : (bool)item.BoolValueField_1;
                model.BoolValueField_2 = item.BoolValueField_2 == null ? false : (bool)item.BoolValueField_2;
                model.BoolValueField_3 = item.BoolValueField_3 == null ? false : (bool)item.BoolValueField_3;
                model.Collection = item.Collection;
                return View(model);
            }

            return RedirectToAction("MyCollection", "Collection");
        }

        [HttpPost]
        public IActionResult Edit(ItemEditModel model)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Detail", "Item", new { id = model.Id });

            Collection collection = _applicationRepository.GetCollectionById(model.Collection.Id);
            if (collection == null)
                return RedirectToAction("Index", "Collection");

            if (model.Name == null || model.Name.Trim() == String.Empty)
            {
                ModelState.AddModelError("Name", _localizer["EmptyNameError"]);
                model.Collection = collection;
                model.AutocompledTags = _applicationRepository.GetAllTags().Select(t => t.Name).ToList();
                return View(model);
            }

            List<string> tagsNames = _applicationRepository.GetAllTagsNames().ToList();
            
            if (collection == null)
                return RedirectToAction("Index", "Collection");

            CollectionItem item = _applicationRepository.GetItemById(model.Id);
            if (item == null)
                return RedirectToAction("Index", "Collection");

            item.Name = model.Name;
            item.Collection = collection;
            List<Tag> itemTags = new List<Tag>();
            if (model.Tags != null)
            {
                for (int i = 0; i < model.Tags.Count(); i++)
                    model.Tags[i] = model.Tags[i].Replace(' ', '_');

                foreach (string tag in model.Tags)
                {
                    if (tag != null && !tagsNames.Contains(tag))
                        _applicationRepository.AddTag(new Tag() { Name = tag });
                }

                List<Tag> tags = _applicationRepository.GetAllTags().ToList();
                for (int i = 0; i < tags.Count; i++)
                {
                    if (model.Tags.Contains(tags[i].Name) && !itemTags.Contains(tags[i]))
                    {
                        itemTags.Add(tags[i]);
                    }
                }
            }

            item.Tags = itemTags;
            if (collection.NumberNameField_1 != null &&
                collection.NumberNameField_1 != String.Empty)
                item.NumberValueField_1 = model.NumberValueField_1;

            if (collection.NumberNameField_2 != null &&
                collection.NumberNameField_2 != String.Empty)
                item.NumberValueField_2 = model.NumberValueField_2;

            if (collection.NumberNameField_3 != null &&
                collection.NumberNameField_3 != String.Empty)
                item.NumberValueField_3 = model.NumberValueField_3;

            if (collection.StringNameField_1 != null &&
                collection.StringNameField_1 != String.Empty)
                item.StringValueField_1 = model.StringValueField_1;

            if (collection.StringNameField_2 != null &&
                collection.StringNameField_2 != String.Empty)
                item.StringValueField_2 = model.StringValueField_2;

            if (collection.StringNameField_3 != null &&
                collection.StringNameField_3 != String.Empty)
                item.StringValueField_3 = model.StringValueField_3;

            if (collection.TextNameField_1 != null &&
                collection.TextNameField_1 != String.Empty)
                item.TextValueField_1 = model.TextValueField_1;

            if (collection.TextNameField_2 != null &&
                collection.TextNameField_2 != String.Empty)
                item.TextValueField_2 = model.TextValueField_2;

            if (collection.TextNameField_3 != null &&
                collection.TextNameField_3 != String.Empty)
                item.TextValueField_3 = model.TextValueField_3;


            TimeOnly timeOnly = new TimeOnly(0, 0, 0);

            if (collection.DateNameField_1 != null &&
                collection.DateNameField_1 != String.Empty &&
                model.DateValueField_1 != null)
            {
                if (collection.IsHasTime_1 && model.TimeValueField_1 != null)
                {
                    item.DateValueField_1 = DateOnly.FromDateTime((DateTime)model.DateValueField_1).ToDateTime(TimeOnly.FromDateTime((DateTime)model.TimeValueField_1));
                }
                else
                {
                    item.DateValueField_1 = DateOnly.FromDateTime((DateTime)model.DateValueField_1).ToDateTime(timeOnly);
                }
            }


            if (collection.DateNameField_2 != null &&
                collection.DateNameField_2 != String.Empty &&
                model.DateValueField_2 != null)
            {
                if (collection.IsHasTime_2 && model.TimeValueField_2 != null)
                {
                    item.DateValueField_2 = DateOnly.FromDateTime((DateTime)model.DateValueField_2).ToDateTime(TimeOnly.FromDateTime((DateTime)model.TimeValueField_2));
                }
                else
                {
                    item.DateValueField_2 = DateOnly.FromDateTime((DateTime)model.DateValueField_2).ToDateTime(timeOnly);
                }
            }

            if (collection.DateNameField_3 != null &&
                collection.DateNameField_3 != String.Empty &&
                model.DateValueField_3 != null)
            {
                if (collection.IsHasTime_3 && model.TimeValueField_3 != null)
                {
                    item.DateValueField_3 = DateOnly.FromDateTime((DateTime)model.DateValueField_3).ToDateTime(TimeOnly.FromDateTime((DateTime)model.TimeValueField_3));
                }
                else
                {
                    item.DateValueField_3 = DateOnly.FromDateTime((DateTime)model.DateValueField_3).ToDateTime(timeOnly);
                }
            }

            if (collection.BoolNameField_1 != null &&
                collection.BoolNameField_1 != String.Empty)
                item.BoolValueField_1 = model.BoolValueField_1;

            if (collection.BoolNameField_2 != null &&
                collection.BoolNameField_2 != String.Empty)
                item.BoolValueField_2 = model.BoolValueField_2;

            if (collection.BoolNameField_3 != null &&
                collection.BoolNameField_3 != String.Empty)
                item.BoolValueField_3 = model.BoolValueField_3;

            _applicationRepository.EditItemCollection(item);

            return RedirectToAction("Index", "Item", new { id = collection.Id });
        }

        public IActionResult Create(int? id)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Item", new { id = id });

            if (id != null)
            {
                Collection collection = _applicationRepository.GetCollectionById((int)id);
                if(collection == null)
                    return RedirectToAction("Index", "Collection");

                if (User.Identity.Name != collection.Owner.UserName && !User.IsInRole("admin"))
                    return RedirectToAction("Index", "Collection");

                ItemCreateModel model = new ItemCreateModel();
                model.AutocompledTags = _applicationRepository.GetAllTags().Select(t => t.Name).ToList();
                model.Collection = collection;
                return View(model);
            }

            return RedirectToAction("Index", "Item");
        }

        [HttpPost]
        public IActionResult Create(ItemCreateModel model)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Item", new { id = model.Collection.Id });

            Collection collection = _applicationRepository.GetCollectionById(model.Collection.Id);
            if (model.Name == null || model.Name.Trim() == String.Empty) 
            {
                ModelState.AddModelError("Name", _localizer["EmptyNameError"]);
                model.AutocompledTags = _applicationRepository.GetAllTags().Select(t => t.Name).ToList();
                model.Collection = collection;
                return View(model);
            }

            List<string> tagsNames = _applicationRepository.GetAllTagsNames().ToList();
            if (collection == null)
                return RedirectToAction("MyCollections", "Collection");

            CollectionItem item = new();
            item.Name = model.Name;
            item.Collection = collection;
            item.CreatedDate = DateTime.Now;
            List<Tag> itemTags = new List<Tag>();
            if (model.Tags != null)
            {
                for(int i = 0; i < model.Tags.Count(); i++)
                    model.Tags[i] = model.Tags[i].Replace(' ', '_');

                foreach (string tag in model.Tags)
                {
                    if (tag != null && !tagsNames.Contains(tag))
                        _applicationRepository.AddTag(new Tag() { Name = tag });
                }

                List<Tag> tags = _applicationRepository.GetAllTags().ToList();
                for (int i = 0; i < tags.Count; i++)
                {
                    if (model.Tags.Contains(tags[i].Name) && !itemTags.Contains(tags[i]))
                    {
                        itemTags.Add(tags[i]);
                    }
                }
            }

            item.Tags = itemTags;
            if (collection.NumberNameField_1 != null &&
                collection.NumberNameField_1 != String.Empty)
                item.NumberValueField_1 = model.NumberValueField_1;

            if (collection.NumberNameField_2 != null &&
                collection.NumberNameField_2 != String.Empty)
                item.NumberValueField_2 = model.NumberValueField_2;

            if (collection.NumberNameField_3 != null &&
                collection.NumberNameField_3 != String.Empty)
                item.NumberValueField_3 = model.NumberValueField_3;

            if (collection.StringNameField_1 != null &&
                collection.StringNameField_1 != String.Empty)
                item.StringValueField_1 = model.StringValueField_1;

            if (collection.StringNameField_2 != null &&
                collection.StringNameField_2 != String.Empty)
                item.StringValueField_2 = model.StringValueField_2;

            if (collection.StringNameField_3 != null &&
                collection.StringNameField_3 != String.Empty)
                item.StringValueField_3 = model.StringValueField_3;

            if (collection.TextNameField_1 != null &&
                collection.TextNameField_1 != String.Empty)
                item.TextValueField_1 = model.TextValueField_1;

            if (collection.TextNameField_2 != null &&
                collection.TextNameField_2 != String.Empty)
                item.TextValueField_2 = model.TextValueField_2;

            if (collection.TextNameField_3 != null &&
                collection.TextNameField_3 != String.Empty)
                item.TextValueField_3 = model.TextValueField_3;

            TimeOnly timeOnly = new TimeOnly(0, 0, 0);

            if (collection.DateNameField_1 != null &&
                collection.DateNameField_1 != String.Empty &&
                model.DateValueField_1 != null)
            {
                if (collection.IsHasTime_1 && model.TimeValueField_1 != null)
                {
                    item.DateValueField_1 = DateOnly.FromDateTime((DateTime)model.DateValueField_1).ToDateTime(TimeOnly.FromDateTime((DateTime)model.TimeValueField_1));
                }
                else
                {
                    item.DateValueField_1 = DateOnly.FromDateTime((DateTime)model.DateValueField_1).ToDateTime(timeOnly);
                }
            }


            if (collection.DateNameField_2 != null &&
                collection.DateNameField_2 != String.Empty &&
                model.DateValueField_2 != null)
            {
                if (collection.IsHasTime_2 && model.TimeValueField_2 != null)
                {
                    item.DateValueField_2 = DateOnly.FromDateTime((DateTime)model.DateValueField_2).ToDateTime(TimeOnly.FromDateTime((DateTime)model.TimeValueField_2));
                }
                else
                {
                    item.DateValueField_2 = DateOnly.FromDateTime((DateTime)model.DateValueField_2).ToDateTime(timeOnly);
                }
            }

            if (collection.DateNameField_3 != null &&
                collection.DateNameField_3 != String.Empty &&
                model.DateValueField_3 != null)
            {
                if (collection.IsHasTime_3 && model.TimeValueField_3 != null)
                {
                    item.DateValueField_3 = DateOnly.FromDateTime((DateTime)model.DateValueField_3).ToDateTime(TimeOnly.FromDateTime((DateTime)model.TimeValueField_3));
                }
                else
                {
                    item.DateValueField_3 = DateOnly.FromDateTime((DateTime)model.DateValueField_3).ToDateTime(timeOnly);
                }
            }

            if (collection.BoolNameField_1 != null &&
                collection.BoolNameField_1 != String.Empty)
                item.BoolValueField_1 = model.BoolValueField_1;

            if (collection.BoolNameField_2 != null &&
                collection.BoolNameField_2 != String.Empty)
                item.BoolValueField_2 = model.BoolValueField_2;

            if (collection.BoolNameField_3 != null &&
                collection.BoolNameField_3 != String.Empty)
                item.BoolValueField_3 = model.BoolValueField_3;

            _applicationRepository.AddItemCollection(item);
            return RedirectToAction("Index", "Item", new { id = collection.Id });
        }

        public IActionResult Delete(int? id)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Detail", "Item", new { id = id });

            if (id != null)
            {
                CollectionItem item = _applicationRepository.GetItemById((int)id);
                if(item == null)
                    return RedirectToAction("Index", "Collection");

                if (User.Identity.Name != item.Collection.Owner.UserName && !User.IsInRole("admin"))
                    return RedirectToAction("Index", "Collection");

                ItemDeleteModel model = new ItemDeleteModel();
                model.Name = item.Name;
                model.Id = item.Id;
                model.CreatedDate = item.CreatedDate;
                model.CollectionId = item.Collection.Id;
                return View(model);
            }

            return RedirectToAction("MyCollection", "Collection");
        }

        [HttpGet]
        public IActionResult AddTag(int? id)
        {
            return PartialView("_AddTag", (int)id);
        }

        [HttpGet]
        public IActionResult TagSearch(string? term, int id)
        {
            List<string> tags = new List<string>();
            if(term != null)
                tags = _applicationRepository.GetAllTagsNames().Where(t => t.Contains(term)).ToList();

            return PartialView("_TagResult", (tags, id));
        }

        [HttpPost]
        public IActionResult Delete(ItemDeleteModel model)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Detail", "Item", new { id = model.Id });

            _applicationRepository.DeleteItemCollectionById(model.Id);
            return RedirectToAction("Index", "Item", new {id = model.CollectionId});
        }

        public IActionResult DeleteComment(int? id)
        {
            if (LogoutAfterBlocked())
                return RedirectToAction("Index", "Home");

            if(id == null)
                return RedirectToAction("Index", "Home");

            Comment comment = _applicationRepository.GetCommentById((int)id);
            if(comment == null)
                return RedirectToAction("Index", "Home");

            if(comment.CollectionItem.Collection.Owner.UserName != User.Identity.Name && !User.IsInRole("admin"))
                return RedirectToAction("Index", "Home");

            return View(comment);
        }

        [HttpPost]
        public IActionResult DeleteComment(Comment comment)
        {
            if(comment == null)
                return RedirectToAction("Index", "Home");

            comment = _applicationRepository.GetCommentById(comment.Id);
            if(comment == null)
                return RedirectToAction("Index", "Home");

            if (comment.CollectionItem.Collection.Owner.UserName != User.Identity.Name && !User.IsInRole("admin"))
                return RedirectToAction("Detail", "Item", new {id = comment.CollectionItem.Id});

            _applicationRepository.DeleteCommentById(comment.Id);
            return RedirectToAction("Detail", "Item", new { id = comment.CollectionItem.Id });
        }

        [Authorize]
        [HttpPost]
        public void LikeDislikeItem(int id)
        {
            var item = _applicationRepository.GetItemById(id);
            var user = _applicationRepository.GetUserByName(User.Identity.Name);
            if (item.LikedUsers.Contains(user))
            {
                item.LikedUsers.Remove(user);
                user.LikedCollections.Remove(item);
                _applicationRepository.EditItemCollection(item);
                _userManager.UpdateAsync(user);
            }
            else
            {
                item.LikedUsers.Add(user);
                if(user.LikedCollections == null)
                    user.LikedCollections = new List<CollectionItem>();

                user.LikedCollections.Add(item);
                _applicationRepository.EditItemCollection(item);
                _userManager.UpdateAsync(user);
            }
        }

        private bool LogoutAfterBlocked()
        {
            if (User.Identity.IsAuthenticated && _applicationRepository.GetBlockUserStatus(User.Identity.Name))
            {
                _signInManager.SignOutAsync();
                return true;
            }

            return false;
        }
    }
}
