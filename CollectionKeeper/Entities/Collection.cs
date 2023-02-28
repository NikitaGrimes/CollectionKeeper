namespace CollectionKeeper.Entities
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; } = String.Empty;

        public Topic Topic { get; set; }

        public string Image { get; set; }

        public string NumberNameField_1 { get; set; } = String.Empty;

        public string NumberNameField_2 { get; set; } = String.Empty;

        public string NumberNameField_3 { get; set; } = String.Empty;

        public string StringNameField_1 { get; set; } = String.Empty;

        public string StringNameField_2 { get; set; } = String.Empty;

        public string StringNameField_3 { get; set; } = String.Empty;

        public string TextNameField_1 { get; set; } = String.Empty;

        public string TextNameField_2 { get; set; } = String.Empty;

        public string TextNameField_3 { get; set; } = String.Empty;

        public string DateNameField_1 { get; set; } = String.Empty;

        public bool IsHasTime_1 { get; set; }

        public string DateNameField_2 { get; set; } = String.Empty;

        public bool IsHasTime_2 { get; set; }

        public string DateNameField_3 { get; set; } = String.Empty;

        public bool IsHasTime_3 { get; set; }

        public string BoolNameField_1 { get; set; } = String.Empty;

        public string BoolNameField_2 { get; set; } = String.Empty;

        public string BoolNameField_3 { get; set; } = String.Empty;

        public CollectionUser Owner { get; set; }

        public ICollection<CollectionItem> Items { get; set; }
    }
}
