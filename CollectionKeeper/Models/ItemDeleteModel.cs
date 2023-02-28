namespace CollectionKeeper.Models
{
    public class ItemDeleteModel
    {
        public int Id { get; set; }

        public int CollectionId { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
