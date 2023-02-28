using CollectionKeeper.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionKeeper.Models
{
    public class CollectionCreatedModel
    {
        public string ownerName { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
        
        public IFormFile Image { get; set; }

        public string Topic { get; set; }

        public List<SelectListItem> Topics { get; set; } = new List<SelectListItem>();

        public string? NumberNameField_1 { get; set; }

        public string? NumberNameField_2 { get; set; }

        public string? NumberNameField_3 { get; set; }

        public string? StringNameField_1 { get; set; }

        public string? StringNameField_2 { get; set; }

        public string? StringNameField_3 { get; set; }

        public string? TextNameField_1 { get; set; }

        public string? TextNameField_2 { get; set; }

        public string? TextNameField_3 { get; set; }

        public string? DateNameField_1 { get; set; }

        public bool IsHasTime_1 { get; set; }

        public string? DateNameField_2 { get; set; }

        public bool IsHasTime_2 { get; set; }

        public string? DateNameField_3 { get; set; }

        public bool IsHasTime_3 { get; set; }

        public string? BoolNameField_1 { get; set; }

        public string? BoolNameField_2 { get; set; }

        public string? BoolNameField_3 { get; set; }

        public bool IsEnabledNumberField_1 { get; set; }

        public bool IsEnabledNumberField_2 { get; set; }

        public bool IsEnabledNumberField_3 { get; set; }

        public bool IsEnabledStringField_1 { get; set; }

        public bool IsEnabledStringField_2 { get; set; }

        public bool IsEnabledStringField_3 { get; set; }

        public bool IsEnabledTextField_1 { get; set; }

        public bool IsEnabledTextField_2 { get; set; }

        public bool IsEnabledTextField_3 { get; set; }

        public bool IsEnabledDateField_1 { get; set; }

        public bool IsEnabledDateField_2 { get; set; }

        public bool IsEnabledDateField_3 { get; set; }

        public bool IsEnabledBoolField_1 { get; set; }

        public bool IsEnabledBoolField_2 { get; set; }

        public bool IsEnabledBoolField_3 { get; set; }
    }
}
