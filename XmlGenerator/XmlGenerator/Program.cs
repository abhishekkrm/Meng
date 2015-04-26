using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                System.Console.WriteLine("InCorrect Arguments. Usage: XMLGenerator.exe <ieee_model_file> <cmponent_input_file>");
                return;
            }

            XMLWriter writer = new XMLWriter(args[0], args[1]);
            writer.GenerateFiles("busDetails.txt", "busMap.txt", "Component.xml");
        }
    }
}
