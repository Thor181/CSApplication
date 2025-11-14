namespace CSLibrary.V2.Stuff
{
    public class DataInterpreter
    {
        private string _data;
        public string Data
        {
            get { return _data; }
            set { _data = value; ParseData(); }
        }

        private Dictionary<string, string> _keyValus = new();

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
            var asSpan = _keyValus[TimeKey].AsSpan();
            var separatorIndex = asSpan.IndexOf('T');
            var date = asSpan[..separatorIndex];
            var time = asSpan[(separatorIndex + 1)..];

            var year = int.Parse(date[..4]);
            var month = int.Parse(date[4..6]);
            var day = int.Parse(date[6..8]);

            var hour = int.Parse(time[..2]);
            var minute = int.Parse(time[2..4]);
            var second = 0;

            if (time.Length == 6)
                second = int.Parse(time[4..6]);

            return new DateTime(year, month, day, hour, minute, second);
        }

        public decimal GetSum() => decimal.Parse(_keyValus[SumKey].Replace('.', ','));

        public string GetFNNumber() => _keyValus[FNKey];

        public string GetFPNumber() => _keyValus[FPKey];
    }
}
