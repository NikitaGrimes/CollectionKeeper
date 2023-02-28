namespace CollectionKeeper.Entities
{
    public class Tag
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<CollectionItem> Items { get; set; }
    }
}
