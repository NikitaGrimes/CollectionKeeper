using Microsoft.EntityFrameworkCore;

namespace CollectionKeeper.Entities
{
    public class CollectionItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Precision(15, 5)]
        public decimal? NumberValueField_1 { get; set;}

        [Precision(15, 5)]
        public decimal? NumberValueField_2 { get; set; }

        [Precision(15, 5)]
        public decimal? NumberValueField_3 { get; set; }

        public string? StringValueField_1 { get; set; } = String.Empty;

        public string? StringValueField_2 { get; set; } = String.Empty;

        public string? StringValueField_3 { get; set; } = String.Empty;

        public string? TextValueField_1 { get; set; } = String.Empty;

        public string? TextValueField_2 { get; set; } = String.Empty;

        public string? TextValueField_3 { get; set; } = String.Empty;

        public DateTime? DateValueField_1 { get; set; }

        public DateTime? DateValueField_2 { get; set; }

        public DateTime? DateValueField_3 { get; set; }

        public bool? BoolValueField_1 { get; set; }

        public bool? BoolValueField_2 { get; set; }

        public bool? BoolValueField_3 { get; set; }

        public Collection Collection { get; set; }

        public ICollection<CollectionUser> LikedUsers { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
