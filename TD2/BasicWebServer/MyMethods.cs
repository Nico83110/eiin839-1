using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer
{
    internal class MyMethods
    {
        public string method1(string var1, string var2)
        {
            return $"<html><body>Voici variable1: {var1} et variable2: {var2}</body></html>";
        }
    }
}
