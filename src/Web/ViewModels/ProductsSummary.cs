using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
