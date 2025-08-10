

using CSSourceGenerator.Stucts;
using Microsoft.CodeAnalysis.Text;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace CSSourceGenerator
{
    public static class Code
    {
        public static SourceText SourceTextUtf8(string code) => SourceText.From(code, Encoding.UTF8);

        public const string ConfigurationAttributeNamespace = "CSApp.V2a.Generated.Configuration";
        public const string ConfigurationAttributeName = "ConfigurationAttribute";
        public const string ConfigurationAttributeFullName = ConfigurationAttributeNamespace + "." + ConfigurationAttributeName;

        public const string ConfigurationAttribute = $@"
namespace {ConfigurationAttributeNamespace}
{{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class {ConfigurationAttributeName} : System.Attribute
    {{
        public readonly string Section;
        public readonly string[] CustomConversion;        

        public ConfigurationAttribute(string section, string[] customConversion = null)
        {{
            Section = section;
            CustomConversion = customConversion;
        }}
    }}
}}
";

        public static string ConfigurationClass(ConfigurationToGenerate model, SectionToGenerate sectionToGenerate)
        {
            var sb = new StringBuilder();

            foreach (var item in sectionToGenerate.Propeties)
            {
                var customConversion = model.CustomConversion.FirstOrDefault(x => x.PropertyName == item.Name);
                var type = item.Type;

                if (customConversion != default && customConversion.Type != type)
                    type = customConversion.Type;

                sb.AppendLine($"\t\tpublic {type} {item.Name} {{get; set;}}");
            }

            return $@"
namespace {model.Namespace}
{{
    public partial class {model.Name} : Microsoft.Extensions.Options.IOptions<{model.Name}>
    {{
        public {model.Name} Value => this;
{sb}
    }}
}}
";
        }

    }
}
