namespace CSLibrary.Stuff
{
    public class Constants
    {
        public const string AtTerritoryPlaceName = "На территории";
        public const string OutTerritoryPlaceName = "За территорией";

        public const string EntranceTypeName = "Вход";
        public const string ExitTypeName = "Выход";

        public const string EmptyPayType = "-";

        public static string MainFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MfRA", "CSApplication");
    }
}
