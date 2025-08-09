using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;

namespace CSLibrary.V2.Data.Logic
{
    public class CardEventLogic : BaseLogic
    {

        public BaseResult WriteCardEvent(int typeId, int pointId, string card)
        {
            var cardEvent = new CardEvent
            {
                Dt = DateTime.Now,
                TypeId = typeId,
                PointId = pointId,
                Card = card
            };

            var result = base.Add(cardEvent);

            return result;
        }
    }
}
