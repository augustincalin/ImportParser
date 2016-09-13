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
            if (args.Length < 2)
            {
                Console.WriteLine("Specify the path of the root CSS file and the desired level of details!");
            }
            else
            {
                Machine machine = new Machine(args[0]);
                string levelOfDetails = args[1];
                Action<string, string, string, int> actionToExecute = null;

                switch (levelOfDetails)
                {
                    case "1":
                        actionToExecute = (name, path, relPath, indent) => { Console.WriteLine(string.Format("{0,-40}", " ".PadLeft(indent * 3) + name)); };
                        break;
                    case "2":
                        actionToExecute = (name, path, relPath, indent) => { Console.WriteLine(string.Format("{0,-30} - {1,-20}", " ".PadLeft(indent * 3) + name, relPath)); };
                        break;
                    case "3":
                    default:
                        actionToExecute = (name, path, relPath, indent) => { Console.WriteLine(string.Format("{0,-20} - {1,-20} - {2,10}", " ".PadLeft(indent * 3) + name, relPath, path)); };
                        break;

                }


                machine.Process(actionToExecute);
            }
        }
    }
}
