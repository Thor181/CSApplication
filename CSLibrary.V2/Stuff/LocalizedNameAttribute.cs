namespace CSLibrary.V2.Stuff
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class LocalizedNameAttribute : Attribute
    {
        public string LocalizedName { get; private set; }

        public LocalizedNameAttribute(string localizedName)
        {
            LocalizedName = localizedName;
        }
    }
}
