using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            var names = JsonConvert.DeserializeObject<IEnumerable<string>>(context.AdditionalFiles[0].GetText().ToString(), _jsonSerializerOptions);
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
