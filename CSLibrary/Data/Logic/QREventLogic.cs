using CSLibrary.Data.Models;
using CSLibrary.Stuff.Results;
using System;

namespace CSLibrary.Data.Logic
{
    public class QREventLogic : BaseLogic
    {
        public BaseResult WriteQREvent(int typeId, decimal sum, string fn, string fp, int pointId, int payType)
        {
            var qrEvent = new Qrevent();

            qrEvent.Id = Guid.NewGuid();
            qrEvent.Dt = DateTime.Now;
            qrEvent.TypeId = typeId;
            qrEvent.PointId = pointId;
            qrEvent.PayId = payType;
            qrEvent.Sum = sum;
            qrEvent.Fn = fn;
            qrEvent.Fp = fp;

            var result = base.Add(qrEvent);
            
            return result;
        }
    }
}
