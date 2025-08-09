using System.Text;

namespace CSLibrary.V2.Stuff.Results
{
    public class BaseResult 
    {
        public bool IsSuccess { get; set; } = true;
        public StringBuilder MessageBuilder { get; private set; } = new StringBuilder();
    }
}
