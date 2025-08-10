using CSLibrary.V2.Data.Interfaces;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;
using System.Linq.Expressions;

namespace CSLibrary.V2.Data.Logic
{
    public class BaseLogic : IDisposable
    {
        private bool _disposed = false;

        protected MfraDbContext DbContext { get; set; }

        public bool DbAvailable { get => DbContext.Database.CanConnect(); }

        public BaseLogic(MfraDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual DbResult<T> Add<T>(T entity) where T : class
        {
            var result = new DbResult<T>();

            if (!DbAvailable)
            {
                result.IsSuccess = false;
                result.DbAvailable = false;

                return result;
            }

            try
            {
                DbContext.Add(entity);
                DbContext.SaveChanges();

                result.MessageBuilder.AppendLine($"Сущность {typeof(T).Name} успешно добавлена");

                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine($"При добавлении сущности ({typeof(T).Name}) в базу данных возникла ошибка | {e.Message} | {e.InnerException}");

                return result;
            }
        }

        public virtual DbResult<IQueryable<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : class, IDbEntity
        {
            var result = new DbResult<IQueryable<T>>();

            if (!DbAvailable)
            {
                result.IsSuccess = false;
                result.DbAvailable = false;

                return result;
            }

            result.Entity = DbContext.Set<T>().Where(predicate);

            return result;
        }

        public virtual BaseResult SaveChanges()
        {
            var result = new BaseResult();

            try
            {
                DbContext.SaveChanges();

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine(e.Message);
            }

            return result;
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                    _disposed = true;
                }
            }
        }
        #endregion
    }
}
