﻿using CSLibrary.Data.Interfaces;
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
                _dbContext.Add(entity);
                _dbContext.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine($"При добавлении сущности ({nameof(T)}) в базу данных возникла ошибка | {e.Message}");

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
