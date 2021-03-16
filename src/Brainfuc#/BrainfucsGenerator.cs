using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;

namespace Brainfucs
{
    [Generator]
    class BrainfuckGenerator : ISourceGenerator
    {
        private const string _template = @"using static System.Console;
namespace Brainfucs
{{
    public static partial class Brainfuck
    {{
        public static unsafe void {0}()
        {{
            int* ptr = stackalloc int[30000];
            {1}
        }}
    }}
}}";

        private static readonly Dictionary<char, string> _lookup = new Dictionary<char, string>()
        {
            ['>'] = "++ptr;",

            ['<'] = "--ptr;",

            ['+'] = "(*ptr)++;",

            ['-'] = "(*ptr)--;",

            ['.'] = "Write((char)*ptr);",

            [','] = "*ptr=(int)ReadKey().Key;",

            ['['] = "while(*ptr!=0){",

            [']'] = "}",
        };

        public void Execute(GeneratorExecutionContext context)
        {
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                string ext = Path.GetExtension(file.Path);
                if (ext != ".b" && ext != ".bf")
                {
                    continue;
                }

                string text = File.ReadAllText(file.Path);
                string fileName = Path.GetFileNameWithoutExtension(file.Path);

                List<string> values = new List<string>(text.Length);
                foreach (char c in text)
                {
                    if (_lookup.TryGetValue(c, out string s))
                    {
                        values.Add(s);
                    }
                }

                context.AddSource($"{fileName}.g", string.Format(_template, fileName, string.Concat(values)));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
