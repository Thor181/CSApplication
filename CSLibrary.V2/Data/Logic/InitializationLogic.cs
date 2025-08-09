using CSLibrary.V2.Data.Interfaces;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;

namespace CSLibrary.V2.Data.Logic
{
    public class InitializationLogic : BaseLogic
    {
        public BaseResult InitializePayTypes()
        {
            var result = InitializeEntity(new PayType() { Name = "-" });

            return result;
        }

        private BaseResult InitializeEntity<T>(T entity) where T : class, IDbEntity, IHelperEntity
        {
            var result = new BaseResult();

            var entityExists = base.Get<T>(x => x.Name == entity.Name)?.Entity?.Any() == true;
            if (entityExists)
            {
                result.MessageBuilder.AppendLine($"Инициализация сущности {typeof(T).Name} не требуется (запись уже существует)");

                return result;
            }

            var dbResult = base.Add(entity);

            result = dbResult;

            return result;
        }
    }
}
