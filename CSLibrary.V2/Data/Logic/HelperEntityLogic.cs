using CSLibrary.V2.Data.Interfaces;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;
using System.Linq.Expressions;

namespace CSLibrary.V2.Data.Logic
{
    public class HelperEntityLogic<T> : BaseLogic where T : class, IHelperEntity
    {
        public HelperEntityLogic(MfraDbContext dbContext) : base(dbContext)
        {
        }

        public DbResult<IQueryable<T>> Get(Expression<Func<T, bool>> predicate)
        {
            var result = new DbResult<IQueryable<T>>();

            var entities = DbContext.Set<T>().Where(predicate);

            if (!DbAvailable)
            {
                result.DbAvailable = false;
                result.IsSuccess = false;

                return result;
            }

            result.Entity = entities;

            return result;
        }
    }
}
