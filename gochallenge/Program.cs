using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace gochallenge
{
    class Program
    {
        private static string[] files = { "pattern_1.splice", "pattern_2.splice", "pattern_3.splice", "pattern_4.splice", "pattern_5.splice" };
        static void Main(string[] args)
        {
            foreach(string s in files) 
            {
                BinaryLoader loader = new BinaryLoader(s);
                if (loader.Open())
                {
                    Sample result = loader.Parse();
                    Console.WriteLine(result);
                }
            }
            Console.Read();
        }
    }
}
