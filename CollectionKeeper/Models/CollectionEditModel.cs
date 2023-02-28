namespace CollectionKeeper.Models
{
    public class CollectionEditModel : CollectionCreatedModel
    {
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public bool IsDeleteImage { get; set; } = false;
    }
}
