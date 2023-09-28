using CSLibrary.Data.Logic;
using CSLibrary.Data.Models;
using CSLibrary.Stuff.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Stuff
{
    public class PersistentValues
    {
        public Place AtTerritoryPlace { get; private set; }
        public Place OutTerritoryPlace { get; private set; }

        public BaseResult Initialize()
        {
            var result = new BaseResult();

            using var helperLogic = new HelperEntityLogic<Place>();

            

            AtTerritoryPlace = baseResult.Entity;

            baseResult = helperLogic.Get(x => x.Name == Constants.OutTerritoryPlace);

            if (!baseResult.IsSuccess)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine(baseResult.MessageBuilder.ToString());

                return result;
            }

            if (baseResult.Entity == null)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine($"Сущность \"{nameof(Place)}\" не найдена в базе данных");

                return result;
            }

            return result;

        }

        private BaseResult InitializeInternal()
        {
            var baseResult = helperLogic.Get(x => x.Name == Constants.AtTerritoryPlace);

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

            if (baseResult.Entity == null)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine($"Сущность \"{nameof(Place)}\" не найдена в базе данных");

                return result;
            }
        }
    }
}
