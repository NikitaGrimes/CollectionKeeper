using CollectionKeeper.Data;
using CollectionKeeper.Entities;
using CollectionKeeper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace CollectionKeeper.Controllers
{
    public class CollectionController : Controller
    {
        private readonly SignInManager<CollectionUser> _signInManager;

        private readonly IApplicationRepository _applicationRepository;

        private readonly IWebHostEnvironment _appEnvironment;

        private readonly IStringLocalizer<CollectionController> _localizer;

        public CollectionController(SignInManager<CollectionUser> signInManager, IStringLocalizer<CollectionController> localizer,
            IApplicationRepository applicationRepository, IWebHostEnvironment appEnvironment)
        {
            _signInManager = signInManager;
            _applicationRepository = applicationRepository;
            _appEnvironment = appEnvironment;
            _localizer = localizer;
        }

        public void Upload(IFormFile file)
        {

        }

        public IActionResult Index()
        {
            LogoutAfterBlocked();
            List<Collection> collections = _applicationRepository.GetAllCollections().ToList();
            if (collections == null)
                collections = new List<Collection>();

            return View(collections);
        }

        [Authorize]
        public IActionResult MyCollections(string? userName)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            if (!User.IsInRole("admin") || userName == null)
                userName = User.Identity.Name;


            MyCollectionsModel model = new MyCollectionsModel();
            model.Collections = _applicationRepository.GetCollectionByUserName(userName).ToList();
            if(model.Collections == null)
                model.Collections = new List<Collection>();

            model.OwnerName = userName;
            return View(model);
        }

        [HttpGet]
        public IActionResult FilterMyCollections(string? userName, string? filter, string? sort)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            if (!User.IsInRole("admin") && userName != User.Identity.Name)
                return null;

            List<Collection> collections = _applicationRepository.GetFiltersSortedCollectionsByUserName(filter, sort, userName).ToList();
            if(collections == null)
                collections = new List<Collection>();

            return PartialView("_MyCollections", collections);
        }

        [HttpGet]
        public IActionResult FilterCollections(string? filter, string? sort)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            List<Collection> collections = _applicationRepository.GetFiltersSortedCollections(filter, sort).ToList();
            if(collections == null)
                collections = new List<Collection>();

            return PartialView("_Collections", collections);
        }

        [Authorize]
        public IActionResult Create(string? ownerName)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            if (!User.IsInRole("admin") && User.Identity.Name != ownerName)
                RedirectToAction("MyCollections", "Collection");

            IEnumerable<Topic> topics = _applicationRepository.GetAllTopics();
            if(topics == null)
                topics = new List<Topic>();

            var collectionCreatedModel = new CollectionCreatedModel();
            foreach(var topic in topics)
                collectionCreatedModel.Topics.Add(new SelectListItem { Text = topic.Name, Value = topic.Name });

            return View(collectionCreatedModel);
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            if (id != null)
            {
                Collection collection = _applicationRepository.GetCollectionById((int)id);
                if (collection == null)
                    return RedirectToAction("Index", "Collection");

                if (User.Identity.Name != collection.Owner.UserName && !User.IsInRole("admin"))
                    return RedirectToAction("Index", "Collection");

                CollectionEditModel model = new CollectionEditModel();
                model.Id = collection.Id;
                model.Name = collection.Name;
                model.Description = collection.Description;
                model.Topic = collection.Topic.Name;
                model.ImagePath = collection.Image;
                IEnumerable<Topic> topics = _applicationRepository.GetAllTopics();
                if(topics == null)
                    topics = new List<Topic>();
                
                foreach (var topic in topics)
                    model.Topics.Add(new SelectListItem { Text = topic.Name, Value = topic.Name });

                if(collection.NumberNameField_1 != null && collection.NumberNameField_1 != String.Empty)
                {
                    model.IsEnabledNumberField_1 = true;
                    model.NumberNameField_1 = collection.NumberNameField_1;
                }

                if (collection.NumberNameField_2 != null && collection.NumberNameField_2 != String.Empty)
                {
                    model.IsEnabledNumberField_2 = true;
                    model.NumberNameField_2 = collection.NumberNameField_2;
                }

                if (collection.NumberNameField_3 != null && collection.NumberNameField_3 != String.Empty)
                {
                    model.IsEnabledNumberField_3 = true;
                    model.NumberNameField_3 = collection.NumberNameField_3;
                }

                if (collection.StringNameField_1 != null && collection.StringNameField_1 != String.Empty)
                {
                    model.IsEnabledStringField_1 = true;
                    model.StringNameField_1 = collection.StringNameField_1;
                }

                if (collection.StringNameField_2 != null && collection.StringNameField_2 != String.Empty)
                {
                    model.IsEnabledStringField_2 = true;
                    model.StringNameField_2 = collection.StringNameField_2;
                }

                if (collection.StringNameField_3 != null && collection.StringNameField_3 != String.Empty)
                {
                    model.IsEnabledStringField_3 = true;
                    model.StringNameField_3 = collection.StringNameField_3;
                }

                if (collection.TextNameField_1 != null && collection.TextNameField_1 != String.Empty)
                {
                    model.IsEnabledTextField_1 = true;
                    model.TextNameField_1 = collection.TextNameField_1;
                }

                if (collection.TextNameField_2 != null && collection.TextNameField_2 != String.Empty)
                {
                    model.IsEnabledTextField_2 = true;
                    model.TextNameField_2 = collection.TextNameField_2;
                }

                if (collection.TextNameField_3 != null && collection.TextNameField_3 != String.Empty)
                {
                    model.IsEnabledTextField_3 = true;
                    model.TextNameField_3 = collection.TextNameField_3;
                }

                if (collection.DateNameField_1 != null && collection.DateNameField_1 != String.Empty)
                {
                    model.IsEnabledDateField_1 = true;
                    model.IsHasTime_1 = collection.IsHasTime_1;
                    model.DateNameField_1 = collection.DateNameField_1;
                }

                if (collection.DateNameField_2 != null && collection.DateNameField_2 != String.Empty)
                {
                    model.IsEnabledDateField_2 = true;
                    model.IsHasTime_2 = collection.IsHasTime_2;
                    model.DateNameField_2 = collection.DateNameField_2;
                }

                if (collection.DateNameField_3 != null && collection.DateNameField_3 != String.Empty)
                {
                    model.IsEnabledDateField_3 = true;
                    model.IsHasTime_3 = collection.IsHasTime_3;
                    model.DateNameField_3 = collection.DateNameField_3;
                }

                if (collection.BoolNameField_1 != null && collection.BoolNameField_1 != String.Empty)
                {
                    model.IsEnabledBoolField_1 = true;
                    model.BoolNameField_1 = collection.BoolNameField_1;
                }

                if (collection.BoolNameField_2 != null && collection.BoolNameField_2 != String.Empty)
                {
                    model.IsEnabledBoolField_2 = true;
                    model.BoolNameField_2 = collection.BoolNameField_2;
                }

                if (collection.BoolNameField_3 != null && collection.BoolNameField_3 != String.Empty)
                {
                    model.IsEnabledBoolField_3 = true;
                    model.BoolNameField_3 = collection.BoolNameField_3;
                }

                return View(model);
            }

            return RedirectToAction("Index", "Collection");
        }

        [HttpPost]
        public IActionResult Edit(CollectionEditModel model)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            IEnumerable<Topic> topics1 = _applicationRepository.GetAllTopics();
            if (topics1 == null)
                topics1 = new List<Topic>();

            foreach (var topic1 in topics1)
                model.Topics.Add(new SelectListItem { Text = topic1.Name, Value = topic1.Name });

            Collection collection = _applicationRepository.GetCollectionById(model.Id);
            model.ImagePath = collection.Image;

            bool isError = false;

            if (model.Name == null || model.Name.Trim() == String.Empty)
            {
                ModelState.AddModelError("Name", _localizer["EmptyNameError"]);
                isError = true;
            }

            if (model.Image == null && model.IsDeleteImage)
                collection.Image = "/images/EmptyImageCollection.jpg";

            else if (model.Image != null)
            {
                FileInfo fileInfo = new FileInfo(model.Image.FileName);
                if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".jpeg" ||
                    fileInfo.Extension == ".png" || fileInfo.Extension == ".svg" ||
                    fileInfo.Extension == ".ico" || fileInfo.Extension == ".gif" ||
                    fileInfo.Extension == ".tiff" || fileInfo.Extension == ".eps")
                {
                    collection.Image = "/images/" + Guid.NewGuid() + fileInfo.Extension;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + collection.Image, FileMode.Create))
                    {
                        model.Image.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    ModelState.AddModelError("Image", _localizer["ImageTypeError"]);
                    isError = true;
                }
            }

            collection.Name = model.Name;
            collection.Description = model.Description == null ? String.Empty : model.Description;
            List<Topic> topics = _applicationRepository.GetAllTopics().ToList();
            if (topics == null)
                topics = new List<Topic>();

            Topic topic = new Topic();
            foreach (var topicItem in topics)
            {
                if (topicItem.Name == model.Topic)
                {
                    topic = topicItem;
                    break;
                }
            }

            collection.Topic = topic;
            if (model.IsEnabledNumberField_1 == true)
            {
                if (model.NumberNameField_1 == null ||
                    model.NumberNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("NumberNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.NumberNameField_1 = model.NumberNameField_1;
            }
            else
                collection.NumberNameField_1 = String.Empty;

            if (model.IsEnabledNumberField_2 == true)
            {
                if (model.NumberNameField_2 == null ||
                    model.NumberNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("NumberNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.NumberNameField_2 = model.NumberNameField_2;
            }
            else
                collection.NumberNameField_2 = String.Empty;

            if (model.IsEnabledNumberField_3 == true)
            {
                if (model.NumberNameField_3 == null ||
                    model.NumberNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("NumberNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.NumberNameField_3 = model.NumberNameField_3;
            }
            else
                collection.NumberNameField_3 = String.Empty;

            if (model.IsEnabledStringField_1 == true)
            {
                if (model.StringNameField_1 == null ||
                    model.StringNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("StringNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.StringNameField_1 = model.StringNameField_1;
            }
            else
                collection.StringNameField_1 = String.Empty;

            if (model.IsEnabledStringField_2 == true)
            {
                if (model.StringNameField_2 == null ||
                    model.StringNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("StringNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.StringNameField_2 = model.StringNameField_2;
            }
            else
                collection.StringNameField_2 = String.Empty;

            if (model.IsEnabledStringField_3 == true)
            {
                if (model.StringNameField_3 == null ||
                    model.StringNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("StringNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.StringNameField_3 = model.StringNameField_3;
            }
            else
                collection.StringNameField_3 = String.Empty;

            if (model.IsEnabledTextField_1 == true)
            {
                if (model.TextNameField_1 == null ||
                    model.TextNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("TextNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.TextNameField_1 = model.TextNameField_1;
            }
            else
                collection.TextNameField_1 = String.Empty;

            if (model.IsEnabledTextField_2 == true)
            {
                if (model.TextNameField_2 == null ||
                    model.TextNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("TextNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.TextNameField_2 = model.TextNameField_2;
            }
            else
                collection.TextNameField_2 = String.Empty;

            if (model.IsEnabledTextField_3 == true)
            {
                if (model.TextNameField_3 == null ||
                    model.TextNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("TextNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.TextNameField_3 = model.TextNameField_3;
            }
            else
                collection.TextNameField_3 = String.Empty;

            if (model.IsEnabledDateField_1 == true)
            {
                if (model.DateNameField_1 == null ||
                    model.DateNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("DateNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                {
                    collection.DateNameField_1 = model.DateNameField_1;
                    collection.IsHasTime_1 = model.IsHasTime_1;
                }
            }
            else
                collection.DateNameField_1 = String.Empty;

            if (model.IsEnabledDateField_2 == true)
            {
                if (model.DateNameField_2 == null ||
                    model.DateNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("DateNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                {
                    collection.DateNameField_2 = model.DateNameField_2;
                    collection.IsHasTime_2 = model.IsHasTime_2;
                }
            }
            else
                collection.DateNameField_2 = String.Empty;

            if (model.IsEnabledDateField_3 == true)
            {
                if (model.DateNameField_3 == null ||
                    model.DateNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("DateNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                {
                    collection.DateNameField_3 = model.DateNameField_3;
                    collection.IsHasTime_3 = model.IsHasTime_3;
                }
            }
            else
                collection.DateNameField_3 = String.Empty;

            if (model.IsEnabledBoolField_1 == true)
            {
                if (model.BoolNameField_1 == null ||
                    model.BoolNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("BoolNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.BoolNameField_1 = model.BoolNameField_1;
            }
            else
                collection.BoolNameField_1 = String.Empty;

            if (model.IsEnabledBoolField_2 == true)
            {
                if (model.BoolNameField_2 == null ||
                    model.BoolNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("BoolNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.BoolNameField_2 = model.BoolNameField_2;
            }
            else
                collection.BoolNameField_2 = String.Empty;

            if (model.IsEnabledBoolField_3 == true)
            {
                if (model.BoolNameField_3 == null ||
                    model.BoolNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("BoolNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.BoolNameField_3 = model.BoolNameField_3;
            }
            else
                model.BoolNameField_3 = String.Empty;

            if (isError)
                return View(model);

            _applicationRepository.EditCollection(collection);
            return RedirectToAction("MyCollections", "Collection");
        }

        [HttpPost]
        public IActionResult Create(CollectionCreatedModel model)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            IEnumerable<Topic> topics1 = _applicationRepository.GetAllTopics();
            if (topics1 == null)
                topics1 = new List<Topic>();

            foreach (var topic1 in topics1)
                model.Topics.Add(new SelectListItem { Text = topic1.Name, Value = topic1.Name });

            bool isError = false;

            if (model.Name == null || model.Name.Trim() == String.Empty)
            {
                isError = true;
                ModelState.AddModelError("Name", _localizer["EmptyNameError"]);
            }

            Collection collection = new();
            if (model.Image == null)
                collection.Image = "/images/EmptyImageCollection.jpg";
            else
            {
                FileInfo fileInfo = new FileInfo(model.Image.FileName);
                if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".jpeg" ||
                    fileInfo.Extension == ".png" || fileInfo.Extension == ".svg" ||
                    fileInfo.Extension == ".ico" || fileInfo.Extension == ".gif" ||
                    fileInfo.Extension == ".tiff" || fileInfo.Extension == ".eps")
                {
                    collection.Image = "/images/" + Guid.NewGuid() + fileInfo.Extension;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + collection.Image, FileMode.Create))
                    {
                        model.Image.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    ModelState.AddModelError("Image", _localizer["ImageTypeError"]);
                    isError = true;
                }
            }

            collection.Name = model.Name;
            collection.Description = model.Description == null ? String.Empty : model.Description;
            List<Topic> topics = _applicationRepository.GetAllTopics().ToList();
            Topic topic = new Topic();
            foreach (var topicItem in topics)
            {
                if (topicItem.Name == model.Topic)
                {
                    topic = topicItem;
                    break;
                }
            }

            collection.Topic = topic;
            if (model.IsEnabledNumberField_1 == true)
            {
                if (model.NumberNameField_1 == null || 
                    model.NumberNameField_1.Trim() == String.Empty)
                {
                    isError = true;
                    ModelState.AddModelError("NumberNameField_1", _localizer["EmptyActiveFieldError"]);
                }
                else
                    collection.NumberNameField_1 = model.NumberNameField_1;
            }
            else
                collection.NumberNameField_1 = String.Empty;

            if (model.IsEnabledNumberField_2 == true)
            {
                if (model.NumberNameField_2 == null || 
                    model.NumberNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("NumberNameField_2", _localizer["EmptyActiveFieldError"]);
                    
                }
                else
                    collection.NumberNameField_2 = model.NumberNameField_2;
            }
            else
                collection.NumberNameField_2 = String.Empty;

            if (model.IsEnabledNumberField_3 == true)
            {
                if (model.NumberNameField_3 == null ||
                    model.NumberNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("NumberNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.NumberNameField_3 = model.NumberNameField_3;
            }
            else
                collection.NumberNameField_3 = String.Empty;

            if (model.IsEnabledStringField_1 == true)
            {
                if (model.StringNameField_1 == null ||
                    model.StringNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("StringNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.StringNameField_1 = model.StringNameField_1;
            }
            else
                collection.StringNameField_1 = String.Empty;

            if (model.IsEnabledStringField_2 == true)
            {
                if (model.StringNameField_2 == null ||
                    model.StringNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("StringNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.StringNameField_2 = model.StringNameField_2;
            }
            else
                collection.StringNameField_2 = String.Empty;

            if (model.IsEnabledStringField_3 == true)
            {
                if (model.StringNameField_3 == null ||
                    model.StringNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("StringNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.StringNameField_3 = model.StringNameField_3;
            }
            else
                collection.StringNameField_3 = String.Empty;

            if (model.IsEnabledTextField_1 == true)
            {
                if (model.TextNameField_1 == null ||
                    model.TextNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("TextNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.TextNameField_1 = model.TextNameField_1;
            }
            else
                collection.TextNameField_1 = String.Empty;

            if (model.IsEnabledTextField_2 == true)
            {
                if (model.TextNameField_2 == null ||
                    model.TextNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("TextNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.TextNameField_2 = model.TextNameField_2;
            }
            else
                collection.TextNameField_2 = String.Empty;

            if (model.IsEnabledTextField_3 == true)
            {
                if (model.TextNameField_3 == null ||
                    model.TextNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("TextNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.TextNameField_3 = model.TextNameField_3;
            }
            else
                collection.TextNameField_3 = String.Empty;

            if (model.IsEnabledDateField_1 == true)
            {
                if (model.DateNameField_1 == null ||
                    model.DateNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("DateNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                {
                    collection.DateNameField_1 = model.DateNameField_1;
                    collection.IsHasTime_1 = model.IsHasTime_1;
                }
            }
            else
                collection.DateNameField_1 = String.Empty;

            if (model.IsEnabledDateField_2 == true)
            {
                if (model.DateNameField_2 == null ||
                    model.DateNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("DateNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                {
                    collection.DateNameField_2 = model.DateNameField_2;
                    collection.IsHasTime_2 = model.IsHasTime_2;
                }
            }
            else
                collection.DateNameField_2 = String.Empty;

            if (model.IsEnabledDateField_3 == true)
            {
                if (model.DateNameField_3 == null ||
                    model.DateNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("DateNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                {
                    collection.DateNameField_3 = model.DateNameField_3;
                    collection.IsHasTime_3 = model.IsHasTime_3;
                }
            }
            else
                collection.DateNameField_3 = String.Empty;

            if (model.IsEnabledBoolField_1 == true)
            {
                if (model.BoolNameField_1 == null ||
                    model.BoolNameField_1.Trim() == String.Empty)
                {
                    ModelState.AddModelError("BoolNameField_1", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.BoolNameField_1 = model.BoolNameField_1;
            }
            else
                collection.BoolNameField_1 = String.Empty;

            if (model.IsEnabledBoolField_2 == true)
            {
                if (model.BoolNameField_2 == null ||
                    model.BoolNameField_2.Trim() == String.Empty)
                {
                    ModelState.AddModelError("BoolNameField_2", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.BoolNameField_2 = model.BoolNameField_2;
            }
            else
                collection.BoolNameField_2 = String.Empty;

            if (model.IsEnabledBoolField_3 == true)
            {
                if (model.BoolNameField_3 == null ||
                    model.BoolNameField_3.Trim() == String.Empty)
                {
                    ModelState.AddModelError("BoolNameField_3", _localizer["EmptyActiveFieldError"]);
                    isError = true;
                }
                else
                    collection.BoolNameField_3 = model.BoolNameField_3;
            }
            else
                collection.BoolNameField_3 = String.Empty;

            if (isError)
                return View(model);

            CollectionUser user = _applicationRepository.GetUserByName(model.ownerName);
            if(user == null)
                return RedirectToAction("MyCollections", "Collection");

            collection.Owner = user;
            _applicationRepository.AddCollection(collection);
            return RedirectToAction("MyCollections", "Collection");
        }

        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            if (id != null)
            {
                Collection collection = _applicationRepository.GetCollectionById((int)id);
                if (User.Identity.Name != collection.Owner.UserName && !User.IsInRole("admin"))
                    return RedirectToAction("Index", "Collection");

                CollectionDeleteModel model = new CollectionDeleteModel();
                model.Id = collection.Id;
                model.Name = collection.Name;
                model.Description = collection.Description;
                return View(model);
            }

            return RedirectToAction("MyCollections", "Collection");
        }

        [HttpPost]
        public IActionResult Delete(CollectionDeleteModel model)
        {
            if (LogoutAfterBlocked())
                RedirectToAction("Index", "Collection");

            _applicationRepository.DeleteCollectionById(model.Id);
            return RedirectToAction("MyCollections", "Collection");
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
