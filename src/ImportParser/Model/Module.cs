using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImportParser.Model
{
    public class Module
    {
        public string Name { get; set; }
        public ICollection<Module> Children { get; set; }

        public string FilePath { get; set; }

        public Module(string filePath)
        {
            if (File.Exists(filePath))
            {
                Name = Path.GetFileName(filePath);
                FilePath = filePath;
                Children = new List<Module>();
            } else
            {
                
            }
        }
    }
}
