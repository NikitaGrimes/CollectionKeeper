namespace CollectionKeeper.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        public DateTime CreatedTime { get; set; }

        public string Text { get; set; }

        public CollectionUser Author { get; set; }

        public CollectionItem CollectionItem { get; set; }
    }
}
