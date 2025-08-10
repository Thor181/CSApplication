using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;

namespace CSLibrary.V2.Data.Logic
{
    public class QREventLogic : BaseLogic
    {
        public QREventLogic(MfraDbContext dbContext) : base(dbContext)
        {
        }

        public BaseResult WriteQREvent(int typeId, decimal sum, string fn, string fp, int pointId, int payType)
        {
            var qrEvent = new Qrevent
            {
                Id = Guid.NewGuid(),
                Dt = DateTime.Now,
                TypeId = typeId,
                PointId = pointId,
                PayId = payType,
                Sum = sum,
                Fn = fn,
                Fp = fp
            };

            var result = base.Add(qrEvent);
            
            return result;
        }
    }
}
