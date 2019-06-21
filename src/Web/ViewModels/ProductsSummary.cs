using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ProductsSummary
    {
        [DisplayName("Count of different products")]
        [Range(1, 10000)]
        public int ProductsCount { get; internal set; }
        [DisplayName("Total count")]
        [Range(1, 1000000)]
        public int ItemsCount { get; internal set; }
        [DisplayName("Total amount")]
        [Range(1, 100000000)]
        public decimal Sum { get; internal set; }
    }
}
