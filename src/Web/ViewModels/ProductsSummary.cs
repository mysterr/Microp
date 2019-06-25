using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ProductsSummary
    {
        [DisplayName("Count of different products")]
        public int ProductsCount { get; internal set; }
        [DisplayName("Total count")]
        public int ItemsCount { get; internal set; }
        [DisplayName("Total amount")]
        public decimal Sum { get; internal set; }
    }
}
