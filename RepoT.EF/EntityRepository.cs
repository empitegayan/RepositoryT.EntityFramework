using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepoT.EF
{
    public abstract class EntityRepository<T, TContext> : RepositoryBase<TContext>
        where T : class
        where TContext : class, IDisposable, IEFDataContext
    {
        private readonly IDbSet<T> _dbset;

        protected EntityRepository(IDatabaseFactory<TContext> databaseFactory) :
            base(databaseFactory)
        {
            _dbset = DataContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = Enumerable.AsEnumerable<T>(_dbset.Where(@where));
            foreach (T obj in objects)
                _dbset.Remove(obj);
        }

        public virtual T GetById(long id)
        {
            return _dbset.Find(id);
        }

        public virtual T GetById(string id)
        {
            return _dbset.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Enumerable.ToList<T>(_dbset);
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return Enumerable.ToList<T>(_dbset.Where(@where));
        }

        public virtual T Get(Expression<Func<T, bool>> where)
        {
            return Queryable.FirstOrDefault<T>(_dbset.Where(@where));
        }

        public virtual IQueryable<T> IncludeSubSets(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbset;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
    }
}