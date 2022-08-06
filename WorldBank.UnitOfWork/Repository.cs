using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System.Linq.Expressions;

namespace WorldBank.UnitOfWork
{

    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            this._dbContext = context ?? throw new ArgumentException(nameof(context));
            this._dbSet = _dbContext.Set<T>();
        }
        public PagedList<T> GetPageIndex(string table, string select, int pageIndex, int pageSize, string order, string orderby, string search)
        {
            try
            {
                if (pageIndex <= -1)
                    throw new Exception("page index must be greater than zero.");
                else if (pageSize <= -1)
                    throw new Exception("page size must be greater than zero.");
                else if (string.IsNullOrEmpty(table))
                    throw new Exception("invalid table name.");
                else if (string.IsNullOrEmpty(select))
                    throw new Exception("invalid select fields.");
                else if (string.IsNullOrEmpty(order))
                    throw new Exception("invalid order.");
                else if (string.IsNullOrEmpty(orderby))
                    orderby = "desc";
                else if (string.IsNullOrEmpty(search))
                    search = "1=1";

                var TotalCount = new SqlParameter
                {
                    ParameterName = "@count",
                    DbType = DbType.Int32,
                    Size = 100,
                    Direction = ParameterDirection.Output
                };
                SqlParameter[] Parameters = {
                    new SqlParameter("@table", SqlDbType.NVarChar) { Value = table },
                    new SqlParameter("@select", SqlDbType.NVarChar) { Value = select },
                    new SqlParameter("@search", SqlDbType.NVarChar) { Value = search },
                    new SqlParameter("@orderby", SqlDbType.NVarChar) { Value = orderby },
                    new SqlParameter("@order", SqlDbType.NVarChar) { Value = order },
                    new SqlParameter("@skip", SqlDbType.NVarChar) { Value = pageIndex },
                    new SqlParameter("@take", SqlDbType.NVarChar) { Value = pageSize },
                    TotalCount
                };
                var _source = this.FromSql("sp_GetPagingData", Parameters, CommandType.StoredProcedure);
                var _count = Convert.ToInt32(Parameters[Parameters.Count() - 1].Value);

                return new PagedList<T>(_source, _count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<T> ToQueryable()
        {
            IQueryable<T> query = this._dbSet;
            return query;
        }
        public IEnumerable<T> Includes(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
            , Func<IQueryable<T>, IOrderedQueryable<T>> order = null)
        {
            IQueryable<T> query = this._dbSet;
            if (include != null)
            {
                query = include(query);
            }
            if (expression != null)
            {
                query = query.Where(expression);
            }
            if (order != null)
            {
                query = order(query);
            }
            return query;
        }

        public IEnumerable<T> FromSql(string query, SqlParameter[] Parameters = null)
        {
            if (Parameters == null)
            {
                SqlParameter[] sqlParameters = { };
                return _dbSet.FromSqlRaw<T>(query, sqlParameters);
            }

            return _dbSet.FromSqlRaw<T>(query, Parameters);
        }

        public IEnumerable<T> FromSql(string query, SqlParameter[] Parameters = null, CommandType type = CommandType.Text)
        {
            List<T> lstReturn = new List<T>();
            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = type;
                _dbContext.Database.OpenConnection();

                if (Parameters != null && type == CommandType.StoredProcedure)
                {
                    foreach (var para in Parameters)
                    {
                        command.Parameters.Add(para);
                    }
                }
                var reader = command.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                foreach (System.Data.DataRow row in dataTable.Rows)
                {
                    lstReturn.Add(GetObject<T>(row));
                }

                _dbContext.Database.CloseConnection();
                return lstReturn;
            }
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = this._dbSet;
            return query;
        }

        public IEnumerable<T> GetByCondition(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = this._dbSet;
            return query.Where(expression);
        }

        public IEnumerable<T> Query(Expression<Func<T, bool>> expression, IOrderedQueryable<T> orderBy = null)
        {
            IQueryable<T> query = this._dbSet;
            return query.Where(expression);
        }

        public T Get(object id)
        {
            return this._dbSet.Find(id);
        }

        public T Single(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = this._dbSet;
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null)
                return orderBy(query).FirstOrDefault();
            return query.FirstOrDefault();
        }

        public void Add(T entity)
        {
            this._dbSet.Add(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            this._dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            this._dbSet.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            this._dbSet.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            this._dbSet.Remove(entity);
        }
        public void Delete(IEnumerable<T> entities)
        {
            this._dbSet.RemoveRange(entities);
        }

        public void Save()
        {
            this._dbContext.SaveChanges();
        }

        public void Dispose()
        {
            this._dbContext?.Dispose();
        }

        #region ### GetObject ###
        private I GetObject<I>(System.Data.DataRow dr)
        {
            I obj = (I)Activator.CreateInstance(typeof(I));
            var parentProperties = obj.GetType().GetProperties();
            foreach (var prop in parentProperties)
            {
                try
                {
                    var propertyInstance = prop.GetValue(obj, null);
                    Guid id;
                    bool isValid = false;
                    if (propertyInstance != null && propertyInstance.ToString() != "0")
                        isValid = propertyInstance.GetType() == typeof(Guid);

                    if (propertyInstance != null && propertyInstance.ToString() != "0" && propertyInstance.GetType() != typeof(Guid))
                    {
                        var mainObjectsProperties = prop.PropertyType.GetProperties();
                        foreach (var property in mainObjectsProperties)
                        {
                            try
                            {
                                object dbObject = dr[property.Name];
                                if (dbObject == DBNull.Value)
                                    property.SetValue(propertyInstance, null);
                                else
                                    property.SetValue(propertyInstance, dbObject);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        object dbObject = dr[prop.Name];
                        if (dbObject == DBNull.Value)
                            prop.SetValue(obj, null);
                        else
                            prop.SetValue(obj, dbObject);
                    }
                }
                catch { }
            }
            return obj;
        }
        #endregion
    }
    public interface IRepository<T> where T : class
    {
        PagedList<T> GetPageIndex(string table, string select, int pageIndex, int pageSize, string order, string orderby, string search);
        IQueryable<T> ToQueryable();
        IEnumerable<T> Includes(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, Func<IQueryable<T>, IOrderedQueryable<T>> order = null);

        IEnumerable<T> FromSql(string query, SqlParameter[] Parameters = null);

        IEnumerable<T> FromSql(string query, SqlParameter[] Parameters = null, CommandType type = CommandType.Text);

        IEnumerable<T> GetAll();
        IEnumerable<T> GetByCondition(Expression<Func<T, bool>> expression);
        IEnumerable<T> Query(Expression<Func<T, bool>> expression, IOrderedQueryable<T> orderBy = null);

        T Get(object id);
        T Single(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        void Add(T entity);
        void Add(IEnumerable<T> entities);

        void Update(T entity);
        void Update(IEnumerable<T> entities);

        void Delete(T entity);
        void Delete(IEnumerable<T> entities);

        void Save();
    }

}
