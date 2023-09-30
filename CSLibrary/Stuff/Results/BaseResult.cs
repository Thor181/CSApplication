using System.Text;

namespace CSLibrary.Stuff.Results
{
    public class BaseResult 
    {
        public bool IsSuccess { get; set; } = true;
        public StringBuilder MessageBuilder { get; private set; } = new StringBuilder();
    }
}
