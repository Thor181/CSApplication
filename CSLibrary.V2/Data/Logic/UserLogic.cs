using CSLibrary.V2.Data.Models;
using CSLibrary.V2.Stuff.Results;

namespace CSLibrary.V2.Data.Logic
{
    public class UserLogic : BaseLogic
    {
        public UserLogic(MfraDbContext dbContext) : base(dbContext)
        {
        }

        public DbResult<User> FindUserByCardNumber(string cardNumber)
        {
            var result = new DbResult<User>();

            var baseResult = Get<User>(x => x.Card.ToLower() == cardNumber.ToLower());

            if (!baseResult.DbAvailable)
            {
                result.DbAvailable = false;
                return result;
            }
            
            var user = baseResult.Entity?.SingleOrDefault();

            if (user == null)
            {
                result.IsSuccess = false;
                result.MessageBuilder.AppendLine($"Запись с кодом карты \"{cardNumber}\" не найдена в базе данных");

                return result;
            }

            result.Entity = user;

            return result;
        }
    }
}
