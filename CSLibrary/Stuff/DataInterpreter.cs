using System;
using System.Collections.Generic;

namespace CSLibrary.Stuff
{
    public class DataInterpreter
    {
        private string _data;
        public string Data
        {
            get { return _data; }
            set { _data = value; ParseData(); }
        }

        private Dictionary<string, string> _keyValus = new Dictionary<string, string>();

        private const string TimeKey = "t";
        private const string SumKey = "s";
        private const string FNKey = "fn";
        private const string FPKey = "fp";

        private void ParseData()
        {
            var splittedPairs = Data.Split('&');
            foreach (var pair in splittedPairs)
            {
                var splittedPair = pair.Split('=');
                _keyValus.Add(splittedPair[0].ToLower(), splittedPair[1]);
            }
        }
        
        public DateTime GetDate()
        {
            var splitted = _keyValus[TimeKey].Split('T');
            var date = splitted[0];
            var time = splitted[1];

            var year = int.Parse(date[..4]);
            var month = int.Parse(date[4..6]);
            var day = int.Parse(date[6..8]);

            var hour = int.Parse(time[..2]);
            var minute = int.Parse(time[2..4]);

            return new DateTime(year, month, day, hour, minute, 0);
        }

        public decimal GetSum() => decimal.Parse(_keyValus[SumKey].Replace('.', ','));

        public string GetFNNumber() => _keyValus[FNKey];

        public string GetFPNumber() => _keyValus[FPKey];
    }
}
