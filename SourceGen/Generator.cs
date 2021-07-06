using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace SourceGen
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private static readonly JsonSerializerSettings _jsonSerializerOptions = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public void Initialize(GeneratorInitializationContext context)
        {
            // This has no implementation because there is nothing to do on init and
            // the method is required to satisfy the interface.
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // The SonarScanner for .NET also contributes additional files so locate the data file by name
            var additionalFile = context.AdditionalFiles.Single(x => "data.json".Equals(Path.GetFileName(x.Path), System.StringComparison.OrdinalIgnoreCase));
            var names = JsonConvert.DeserializeObject<IEnumerable<string>>(additionalFile.GetText().ToString(), _jsonSerializerOptions);

            foreach (var name in names)
            {
                string source = $@"
using System;

namespace SonarBuildFailRepro
{{
    public class {name}
    {{
        public void Hello()
        {{
            Console.WriteLine(""Hello World!"");
        }}
    }}
}}";
                context.AddSource($"{name}.cs", source);
            }
        }
    }
}
