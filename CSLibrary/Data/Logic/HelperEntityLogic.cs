﻿using CSLibrary.Data.Interfaces;
using CSLibrary.Stuff.Results;
using System.Linq.Expressions;

namespace CSLibrary.Data.Logic
{
    public class HelperEntityLogic<T> : BaseLogic where T : class, IHelperEntity
    {
        public DbResult<IQueryable<T>> Get(Expression<Func<T, bool>> predicate)
        {
            var result = new DbResult<IQueryable<T>>();

            var entities = _dbContext.Set<T>().Where(predicate);

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
