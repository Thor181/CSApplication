using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.V2.Data.Logic
{
    public class OperatorEventLogic : BaseLogic
    {
        public OperatorEventLogic(MfraDbContext dbContext) : base(dbContext) 
        {
            
        }

        public BaseResult WriteEvent(int typeId, int pointId)
        {
            var operatorEvent = new OperatorEvent()
            {
                PointId = pointId,
                TypeId = typeId,
            };

            var result = base.Add(operatorEvent);
            return result;
        }
    }
}
