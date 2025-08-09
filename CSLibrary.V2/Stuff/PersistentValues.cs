using CSLibrary.V2.Data.Interfaces;
using CSLibrary.V2.Data.Logic;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;

namespace CSLibrary.V2.Stuff
{
    public class PersistentValues
    {
        public Place? AtTerritoryPlace;
        public Place? OutTerritoryPlace;

        public EventsType? Entrance;
        public EventsType? Exit;

        public Point Point;

        public PayType EmptyPayType;

        public PersistentValues()
        {
            
        }

        public BaseResult Initialize()
        {
            var result = new BaseResult();

            using var helperLogic = new HelperEntityLogic<Place>();

            result = InitializeInternal(helperLogic, ref AtTerritoryPlace!, Constants.AtTerritoryPlaceName);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(helperLogic, ref OutTerritoryPlace!, Constants.OutTerritoryPlaceName);

            if (!result.IsSuccess)
                return result;

            using var typeHelperLogic = new HelperEntityLogic<EventsType>();

            result = InitializeInternal(typeHelperLogic, ref Entrance!, Constants.EntranceTypeName);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(typeHelperLogic, ref Exit!, Constants.ExitTypeName);

            if (!result.IsSuccess)
                return result;

            using var pointHelperLogic = new HelperEntityLogic<Point>();

            result = InitializeInternal(pointHelperLogic, ref Point, AppConfig.Instance.PointIdentifier);

            if (!result.IsSuccess)
                return result;

            using var payTypeHelperLogic = new HelperEntityLogic<PayType>();

            result = InitializeInternal(payTypeHelperLogic, ref EmptyPayType, Constants.EmptyPayType);

            if (!result.IsSuccess)
                return result;

            result.MessageBuilder.AppendLine($"{nameof(PersistentValues)} успешно инициализировано");
            return result;
        }

        private BaseResult InitializeInternal<T>(HelperEntityLogic<T> helperLogic, ref T initializableProp, string name) where T : class, IHelperEntity
        {
            var result = new BaseResult();

            var baseResult = helperLogic.Get(x => x.Name == name);

            if (!baseResult.DbAvailable)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine("База данных недоступна");

                return result;
            }

            if (!baseResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine(baseResult.MessageBuilder.ToString());

                return result;
            }

            var entity = baseResult.Entity?.SingleOrDefault();

            if (baseResult.Entity == null || entity == null)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine($"Сущность \"{typeof(T).Name}\" с именем \"{name}\" не найдена в базе данных");

                return result;
            }

            initializableProp = entity;

            return result;
        }
    }
}
