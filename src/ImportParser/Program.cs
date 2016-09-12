using ImportParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImportParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Machine machine = new Machine(@"d:\playground\importparser\root.css");
            machine.Process((name, path, relPath, indent) => { Console.WriteLine(string.Format("{0,-20} - {1,-20} - {2,10}", " ".PadLeft(indent * 3) + name, relPath, path)); });
            Console.ReadLine();
        }
    }
}
