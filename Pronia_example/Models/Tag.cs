using Pronia_example.Models.Common;

namespace Pronia_example.Models
{
    public class Tag : BaseEntity
    {

        public string Name { get; set; }

        public ICollection<ProductTag> ProductTags { get; set; } = [];
    }


}
