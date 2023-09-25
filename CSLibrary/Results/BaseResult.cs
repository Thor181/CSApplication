using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Results
{
    public class BaseResult
    {
        public bool IsSuccses { get; set; } = true;

        public StringBuilder MessageBuilder { get; private set; } = new StringBuilder();
    }
}
