using CSLibrary.Data.Models;
using CSLibrary.Stuff.Results;
using System;

namespace CSLibrary.Data.Logic
{
    public class CardEventLogic : BaseLogic
    {

        public BaseResult WriteCardEvent(int typeId, int pointId, string card)
        {
            var cardEvent = new CardEvent();
            cardEvent.Dt = DateTime.Now;
            cardEvent.TypeId = typeId;
            cardEvent.PointId = pointId;
            cardEvent.Card = card;

            var result = base.Add(cardEvent);

            return result;
        }
    }
}
