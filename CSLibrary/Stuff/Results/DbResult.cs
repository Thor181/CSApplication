using CSLibrary.Data.Interfaces;
using CSLibrary.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLibrary.Stuff.Results
{
    public class DbResult<T> : BaseResult
    {
        public bool DbAvailable { get; set; } = true;
        public T? Entity { get; set; }
    }
}
