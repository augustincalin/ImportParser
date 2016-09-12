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
        private Regex regex = new Regex("@import\\s+(url\\(('|\")(?<name1>.+?)('|\")\\)|('|\")(?<name2>.+?)('|\"))", RegexOptions.Compiled);
        private Module _root;
        public Machine(string fileName)
        {
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

                    foreach (Match match in regex.Matches(fileContent))
                    {
                        string importPath = match.Groups["name1"].Success ? match.Groups["name1"].Value : match.Groups["name2"].Value;
                        string filePath = Path.Combine(Path.GetDirectoryName(module.FilePath), importPath);
                        Module child = new Module(Path.GetFullPath(filePath));
                        module.Children.Add(child);
                        await ProcessModule(child);
                    }
                }
            }
            catch (Exception ex)
            {

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

        public void Process(Action<string, string,string, int> action)
        {
            ProcessModule(_root, 0, action);
        }

    }
}
