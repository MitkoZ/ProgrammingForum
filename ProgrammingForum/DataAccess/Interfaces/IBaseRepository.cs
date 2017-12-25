using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        List<T> GetAll(Func<T, bool> filter = null);

        void Save(T item);

        void Create(T item);

        void Update(T item, Func<T, bool> findByPredicate);
    }
}
