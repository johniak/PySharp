using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PySharpC
{
    class Variable
    {
        public int Size { get; set; }
        public string Name { get; set; }
        public int StackPosition { get; set; }
        public VarType Type { get; set; }
    }

    enum VarType
    {
        integer,
        character,
        str
    }
}
