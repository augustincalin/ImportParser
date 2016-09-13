using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImportParser.Model
{
    public class Machine
    {
        private Dictionary<string, string> _lessVariables;
        private Regex regexImport = new Regex("^@import\\s+(url\\(('|\")(?<name1>.+?)('|\")\\)|('|\")(?<name2>.+?)('|\"))", RegexOptions.Compiled|RegexOptions.Multiline|RegexOptions.IgnoreCase);
        private Regex regexLessVariable = new Regex("^@(?<key>.+?)\\s*:\\s*\"(?<value>.+?)\";", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private Module _root;

        public Machine(string fileName)
        {
            _lessVariables = new Dictionary<string, string>();
            _root = new Module(fileName);
            Task t = StartProcessing();
            t.Wait();
        }

        private async Task<Module> StartProcessing()
        {
            await ProcessModule(_root);
            return _root;
        }

        private async Task ProcessModule(Module module)
        {
            try
            {
                using (StreamReader sr = File.OpenText(module.FilePath))
                {
                    string fileContent = await sr.ReadToEndAsync();
                    ProcessLessVariables(fileContent);

                    await ProcessImports(module, fileContent);
                }
            }
            catch (Exception ex) { }
        }

        private async Task ProcessImports(Module module, string fileContent)
        {
            foreach (Match match in regexImport.Matches(fileContent))
            {
                string importPath = match.Groups["name1"].Success ? match.Groups["name1"].Value : match.Groups["name2"].Value;
                string normalizedImportPath = NormalizeImportPath(importPath);
                string filePath = Path.Combine(Path.GetDirectoryName(module.FilePath), normalizedImportPath);
                
                Module child = new Module(Path.GetFullPath(filePath));
                module.Children.Add(child);
                await ProcessModule(child);
            }
        }

        private string NormalizeImportPath(string importPath)
        {
            foreach(var variable in _lessVariables)
            {
                if (importPath.Contains(variable.Key))
                {
                    importPath = importPath.Replace(variable.Key, variable.Value);
                }
            }
            return importPath;
        }

        private void ProcessLessVariables(string fileContent)
        {
            foreach (Match match in regexLessVariable.Matches(fileContent))
            {
                string key = match.Groups["key"].Value;
                if (key != "import")
                {
                    string value = match.Groups["value"].Value;
                    _lessVariables["@{" + key + "}"] = value;
                }
            }
        }

        private void ProcessModule(Module module, int level, Action<string, string, string, int> action)
        {
            action(module.Name, module.FilePath, module.FilePath.Replace(Path.GetDirectoryName(_root.FilePath) + Path.DirectorySeparatorChar.ToString(), ""), level);
            foreach (Module m in module.Children)
            {
                ProcessModule(m, level + 1, action);
            }
        }

        public void Process(Action<string, string, string, int> action)
        {
            ProcessModule(_root, 0, action);
        }

    }
}
