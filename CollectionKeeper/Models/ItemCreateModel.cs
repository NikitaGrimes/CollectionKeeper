using CollectionKeeper.Entities;

namespace CollectionKeeper.Models
{
    public class ItemCreateModel
    {
        public string Name { get; set; }

        public List<string> Tags { get; set; }

        public List<string> AutocompledTags { get; set; }

        public decimal? NumberValueField_1 { get; set; }

        public decimal? NumberValueField_2 { get; set; }

        public decimal? NumberValueField_3 { get; set; }

        public string? StringValueField_1 { get; set; }

        public string? StringValueField_2 { get; set; }

        public string? StringValueField_3 { get; set; }

        public string? TextValueField_1 { get; set; }

        public string? TextValueField_2 { get; set; }

        public string? TextValueField_3 { get; set; }

        public DateTime? DateValueField_1 { get; set; }

        public DateTime? TimeValueField_1 { get; set; }

        public DateTime? DateValueField_2 { get; set; }

        public DateTime? TimeValueField_2 { get; set; }

        public DateTime? DateValueField_3 { get; set; }

        public DateTime? TimeValueField_3 { get; set; }

        public bool BoolValueField_1 { get; set; }

        public bool BoolValueField_2 { get; set; }

        public bool BoolValueField_3 { get; set; }

        public Collection Collection { get; set; }
    }
}
