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
    public class HelperEntityLogic<T> : BaseLogic where T : class, IHelperEntity
    {
        public DbResult<T> Get(Expression<Func<T, bool>> predicate)
        {
            var result = new DbResult<T>();

            var baseResult = Get(predicate);

            if (!baseResult.DbAvailable)
            {
                result.DbAvailable = false;
                result.IsSuccess = false;

                return result;
            }

            result.Entity = baseResult.Entity;

            return result;
        }
    }
}
