using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        protected ProgrammingForumDbContext Context;

        protected DbSet<T> DBSet
        {
            get
            {
                return Context.Set<T>();
            }
        }

        public BaseRepository() : this(new ProgrammingForumDbContext())
        {

        }

        public BaseRepository(ProgrammingForumDbContext context)
        {
            Context = context;
        }

        public List<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public T GetFirst(Func<T, bool> filter)
        {
            return Context.Set<T>().FirstOrDefault(filter);
        }

        public List<T> GetAll(Func<T, bool> filter)
        {
            return Context.Set<T>().Where(filter).ToList();
        }

        public T GetByID(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public void Create(T item)
        {
            Context.Set<T>().Add(item);
        }
        public void Update(T item, Func<T, bool> findByIDPredicate)
        {
            var local = Context.Set<T>()
                         .Local
                         .FirstOrDefault(findByIDPredicate);
            if (local != null)
            {
                Context.Entry(local).State = EntityState.Detached;
            }
            Context.Entry(item).State = EntityState.Modified;
        }

        public void DeleteByID(int id)
        {
            T dbItem = Context.Set<T>().Find(id);
            if (dbItem != null)
            {
                Context.Set<T>().Remove(dbItem);
            }
        }

        public void DeleteByPredicate(Func<T, bool> filter)
        {
            T dbItem = Context.Set<T>().FirstOrDefault(filter);
            if (dbItem != null)
            {
                Context.Set<T>().Remove(dbItem);
            }
        }

        public void Save(T item)
        {
            if (item.Id == 0)
            {
                Create(item);
            }
            else
            {
                Update(item, x => x.Id == item.Id);
            }
        }
    }
}
