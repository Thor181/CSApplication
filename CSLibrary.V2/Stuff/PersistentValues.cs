using CSLibrary.V2.Data.Interfaces;
using CSLibrary.V2.Data.Logic;
using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;

namespace CSLibrary.V2.Stuff
{
    public class PersistentValues
    {
        private readonly HelperEntityLogic<Place> _helperLogic;
        private readonly HelperEntityLogic<EventsType> _typeHelperLogic;
        private readonly HelperEntityLogic<Point> _pointHelperLogic;
        private readonly HelperEntityLogic<PayType> _payTypeHelperLogic;
        private readonly IPortWorkerOptions _portWorkerOptions;

        private Place? _atTerritoryPlace;
        private Place? _outTerritoryPlace;
        private EventsType? _entrance;
        private EventsType? _exit;
        private Point _point;
        private PayType _emptyPayType;

        public Place? AtTerritoryPlace { get => _atTerritoryPlace; set => _atTerritoryPlace = value; }
        public Place? OutTerritoryPlace { get => _outTerritoryPlace; set => _outTerritoryPlace = value; }
        public EventsType? Entrance { get => _entrance; set => _entrance = value; }
        public EventsType? Exit { get => _exit; set => _exit = value; }
        public Point Point { get => _point; set => _point = value; }
        public PayType EmptyPayType { get => _emptyPayType; set => _emptyPayType = value; }

        public PersistentValues(HelperEntityLogic<Place> helperLogic, 
            HelperEntityLogic<EventsType> typeHelperLogic, 
            HelperEntityLogic<Point> pointHelperLogic, 
            HelperEntityLogic<PayType> payTypeHelperLogic,
            IPortWorkerOptions portWorkerOptions)
        {
            _helperLogic = helperLogic;
            _typeHelperLogic = typeHelperLogic;
            _pointHelperLogic = pointHelperLogic;
            _payTypeHelperLogic = payTypeHelperLogic;
            _portWorkerOptions = portWorkerOptions;
        }

        public BaseResult Initialize()
        {
            var result = new BaseResult();

            result = InitializeInternal(_helperLogic, ref _atTerritoryPlace!, Constants.AtTerritoryPlaceName);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(_helperLogic, ref _outTerritoryPlace!, Constants.OutTerritoryPlaceName);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(_typeHelperLogic, ref _entrance!, Constants.EntranceTypeName);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(_typeHelperLogic, ref _exit!, Constants.ExitTypeName);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(_pointHelperLogic, ref _point, _portWorkerOptions.PointIdentifier);

            if (!result.IsSuccess)
                return result;

            result = InitializeInternal(_payTypeHelperLogic, ref _emptyPayType, Constants.EmptyPayType);

            if (!result.IsSuccess)
                return result;

            result.MessageBuilder.AppendLine($"{nameof(PersistentValues)} успешно инициализировано");
            return result;
        }

        private static BaseResult InitializeInternal<T>(HelperEntityLogic<T> helperLogic, ref T initializableProp, string name) where T : class, IHelperEntity
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
