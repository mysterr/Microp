using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Infrastructure
{
    public interface IRepository<T>
    {
        Task<int> GetCount();
        Task<decimal> GetSum();
        Task<int> GetTotal();
        Task Create(T item);
        Task<IEnumerable<T>> Get(string name);
    }
}
