﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySharpC
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler compiler = new Compiler("main.pys");
            compiler.Open();
            compiler.Compile();
            compiler.SaveASM();
            Process.Start("gcc", @"-o main.exe -m32  .\main.pys.s");
        }
    }
}
