using CSLibrary.V2.Data.Interfaces;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;
using Microsoft.EntityFrameworkCore;

namespace CSLibrary.V2.Data.Logic
{
    public class InitializationLogic : BaseLogic
    {
        public InitializationLogic(MfraDbContext dbContext) : base(dbContext)
        {
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

        public BaseResult InitializePayTypes()
        {
            var result = InitializeEntity(new PayType() { Name = "-" });

            return result;
        }

        public BaseResult InitializeOperatorEventsIfNotExists()
        {
                var baseResult = new BaseResult() { IsSuccess = true };
            try
            {
                var sql = """
                    if not exists (select [name] from sys.tables where [name] = 'OperatorEvents')
                begin 
                	CREATE TABLE OperatorEvents (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Dt DATETIME NOT NULL DEFAULT GETDATE(),
                    TypeId INT NOT NULL,
                    PointId INT NOT NULL,
                    CONSTRAINT FK_OperatorEvents_EventsType FOREIGN KEY (TypeId)
                        REFERENCES EventsTypes(Id) ON DELETE NO ACTION,
                    CONSTRAINT FK_OperatorEvents_Point FOREIGN KEY (PointId)
                        REFERENCES Points(Id) ON DELETE NO ACTION
                )
                end;
                """;


                var result = DbContext.Database.ExecuteSqlRaw(sql);

                if (result == 0)
                    baseResult.MessageBuilder.AppendLine("Таблица 'OperatorEvents' успешно создана");
                else if (result == -1)
                    baseResult.MessageBuilder.AppendLine("Таблица 'OperatorEvents' уже существует");

                baseResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                baseResult.IsSuccess = false;
                baseResult.MessageBuilder.AppendLine($"При создании таблицы 'OperatorEvents' возникла ошибка | {ex}");
            }

            return baseResult;
        }
    }
}
