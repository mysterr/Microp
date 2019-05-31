using System.ComponentModel;

namespace Web.ViewModels
{
    public class ProductsSummary
    {
        [DisplayName("Количество разных товаров")]
        public int ProductsCount { get; internal set; }
        [DisplayName("Общее количество")]
        public int ItemsCount { get; internal set; }
        [DisplayName("Общая сумма")]
        public decimal Sum { get; internal set; }
    }
}
