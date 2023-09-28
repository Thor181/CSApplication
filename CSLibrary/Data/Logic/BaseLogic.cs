using CSLibrary.Data.Interfaces;
using CSLibrary.Data.Models;
using CSLibrary.Stuff.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Data.Logic
{
    public class BaseLogic : IDisposable
    {
        private bool _disposed = false;

        protected MfRadbContext _dbContext { get; set; }

        public bool DbAvailable { get { return _dbContext.Database.CanConnect(); } }

        public BaseLogic()
        {
            _dbContext = new MfRadbContext();
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

            result.Entity = _dbContext.Set<T>().Where(predicate);

            return result;
        }

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
                    _dbContext.Dispose();
            }
        }
    }
}
