using System.Text;

namespace CSLibrary
{
    public class DbConnectionString
    {
        private string _server = "localhost";
        public string Server { get { return _server; } set { _server = value; } }

        private int _port = 1433;
        public int Port { get { return _port; } set { _port = value; } }

        private string _database = "MfRADb";
        public string Database { get { return _database; } set { _database = value; } }

        private string _user = string.Empty;
        public string User { get { return _user; } set { _user = value; } }

        private string _password = string.Empty;
        public string Password { get { return _password; } set { _password = value;  } }

        private bool _trustedConnection = true;
        public bool TrustedConnection { get { return _trustedConnection; } set { _trustedConnection = value; } }

        private bool _encrypt = false;
        public bool Encrypt { get { return _encrypt; } set { _encrypt = value; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Server={Server};");
            sb.Append($"Database={Database};");

            if (TrustedConnection == false)
            {
                sb.Append($"User Id={User};");
                sb.Append($"Password={Password};");
            }
            else
                sb.Append($"Trusted_Connection={TrustedConnection};");

            sb.Append($"Encrypt={Encrypt};");
            sb.Append($"TrustServerCertificate=true;");

            return sb.ToString();
        }
    }
}
