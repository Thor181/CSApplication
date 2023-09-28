using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Stuff.Results
{
    public class BaseResult 
    {
        public bool IsSuccess { get; set; } = true;
        public StringBuilder MessageBuilder { get; private set; } = new StringBuilder();
    }
}
