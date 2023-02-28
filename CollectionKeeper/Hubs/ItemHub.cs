using CollectionKeeper.Data;
using CollectionKeeper.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace CollectionKeeper.Hubs
{
    public class ItemHub : Hub
    {
        private readonly IApplicationRepository _applicationRepository;

        public ItemHub(IApplicationRepository applicationRepository) 
        {
            _applicationRepository = applicationRepository;
        }

        public async Task SendComment(string text, string userName, int id)
        {
            string divId = "commentDiv_" + id;
            CollectionUser user = _applicationRepository.GetUserByName(userName);
            CollectionItem item = _applicationRepository.GetItemById(id);
            Comment comment = new Comment()
            {
                CreatedTime = DateTime.Now,
                Text = text,
                Author = user,
                CollectionItem = item,
            };
            _applicationRepository.AddComment(comment);
            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"form-control\"><p><strong>");
            html.Append(userName);
            html.Append("</strong></p> <p class=\"small text-muted\">");
            html.Append(comment.CreatedTime.ToString());
            html.Append("</p><p>");
            html.Append(text);
            html.Append("</p></div>");
            await Clients.All.SendAsync("SendComment", html.ToString(), divId);
        }

        public async Task SendLike(int number, int id)
        {
            string likeCountId = "likedCount_" + id;
            await Clients.All.SendAsync("SendLike", likeCountId, number);
        }
    }
}
