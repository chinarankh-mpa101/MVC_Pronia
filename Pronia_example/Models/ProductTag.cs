using Pronia_example.Models.Common;

namespace Pronia_example.Models
{
    public class ProductTag : BaseEntity
    {

        public Product Product { get; set; } =null;
        public int ProductId { get; set; }

        public Tag Tag { get; set; }=null;
        public int TagId { get; set; }
    }


}
