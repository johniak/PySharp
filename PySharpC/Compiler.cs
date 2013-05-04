using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace PySharpC
{
    class Compiler
    {
        string filename;
        string file;
        string[] lines;
        List<string> functionNames = new List<string>();
        Dictionary<string, Block> functions = new Dictionary<string, Block>();
        String variableNameRegex = "(?<name>[a-zA-Z]+([a-zA-Z0-9_])*)";
        List<string> assemblyText = new List<string>();
        List<string> assemblyData = new List<string>();
        List<string> basicTypes = new List<string>();
        int currentLine = 0;
        static int constCounter;
        public Compiler(String filename)
        {
            this.filename = filename;
            basicTypes.Add("int");
            basicTypes.Add("char");
            basicTypes.Add("str");
        }

        public void Open()
        {
            var stream = new StreamReader(File.Open(filename, FileMode.Open));
            file = stream.ReadToEnd();
            stream.Close();
            lines = file.Split('\n');
        }

        public void Compile()
        {
            getFunctions();
            for (int i = 0; i < functionNames.Count; i++)
            {
                compileFunction(i);
            }
        }
        private void getFunctions()
        {
            string name = null;
            int start = 0;
            int end = 0;
            bool added = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string argumentsPatern = @"[ ]*\(([ ]*((int)|(char)|(str))[ ]+([a-zA-Z]+([a-zA-Z0-9_])*)[ ]*)?([ ]*,[ ]*((int)|(char)|(str))[ ]+([a-zA-Z]+([a-zA-Z0-9_])*)[ ]*)*\)";
                Match matchDef = Regex.Match(lines[i], @"^def[ ]+" + variableNameRegex + argumentsPatern + @"[ ]*:[\s]*$");
                if (matchDef.Success)
                {
                    name = matchDef.Groups["name"].Value;
                    if (name == "main")
                        name = "_main";
                    start = i;
                    added = false;
                    continue;
                }
                Match matchLine = Regex.Match(lines[i], @"^\t");
                if (!matchLine.Success && name != null)
                {
                    end = i - 1;
                    functionNames.Add(name);
                    functions.Add(name, new Block(start, end));
                    added = true;
                    name = null;
                }
            }
            if (!added && name != null)
            {
                end = lines.Length - 1;
                functionNames.Add(name);
                functions.Add(name, new Block(start, end));
            }
        }
        private void compileFunction(int id)
        {
            string name = functionNames[id];
            Block block = functions[name];
            Dictionary<string, Variable> locals = new Dictionary<string, Variable>();
            int level = 1;
            assemblyText.Add(".global " + name);
            assemblyText.Add(name + ":");
            assemblyText.Add("push %ebp");
            assemblyText.Add("push %ebx");
            assemblyText.Add("mov %esp,%ebp");
            int argsCount = 0;
            string[] arguments = Regex.Match(lines[block.Start], @"\((?<args>.*)\)").Groups["args"].Value.Split(',');
            {
                int i = -3;
                foreach (var arg in arguments)
                {
                    var variable = variableDeclarator(arg);
                    if (variable == null)
                        continue;
                    variable.StackPosition = i--;
                    locals.Add(variable.Name, variable);
                    argsCount++;
                }
            }
            blockCompilator(block, locals, level, argsCount);

            assemblyText.Add("mov %ebp,%esp");
            assemblyText.Add("pop %ebx");
            assemblyText.Add("pop %ebp");
            assemblyText.Add("mov $0,%eax");
            assemblyText.Add("ret");
            assemblyText.Add("");
        }
        void blockCompilator(Block block, Dictionary<string, Variable> locals, int level, int argsCount)
        {
            List<Variable> localLocals = new List<Variable>();
            currentLine = block.Start;
            for (; currentLine <= block.End; )
            {
                Variable var = variableDeclarator(lines[currentLine].Trim());
                if (var != null)
                {
                    var.StackPosition = locals.Count;
                    localLocals.Add(var);
                    locals.Add(var.Name, var);
                    assemblyText.Add("sub $4,%esp #"+var.Name);
                    currentLine++;
                    continue;
                }
                if (Regex.IsMatch(lines[currentLine].Trim(), @"^asm:[\s]*$"))
                {
                    Block newBlock = findBlock(level + 1, currentLine + 1);
                    inlineAssembler(newBlock, locals, argsCount);
                    continue;
                }
                if (isVariableOperiation(lines[currentLine]))
                {
                    variableOperation(lines[currentLine], locals, argsCount);
                    currentLine++;
                    continue;
                }
                if (isFunctionCalling(lines[currentLine].Trim()))
                {
                    functionCalling(lines[currentLine].Trim(), locals, argsCount);
                    currentLine++;
                    continue;
                }
                currentLine++;

            }
            assemblyText.Add("add $" + localLocals.Count *4+ ",%esp");

            foreach (var local in localLocals)
            {
                locals.Remove(local.Name);
            }
        }
        Block findBlock(int level, int line)
        {
            for (int i = line; i < lines.Length; i++)
            {
                if (getLevel(lines[i]) < level)
                    return new Block(line, i - 1);
            }
            return new Block(line, lines.Length);
        }
        void inlineAssembler(Block block, Dictionary<string, Variable> locals, int argsCount)
        {
            currentLine = block.Start;
            for (; currentLine <= block.End; currentLine++)
            {
                String line = lines[currentLine].Trim();
                Match match = Regex.Match(line, @"\^" + variableNameRegex);
                if (match.Success)
                {
                    Variable var = locals[match.Groups["name"].Value];
                    line = Regex.Replace(line, @"\^" + variableNameRegex, buildStackVariable(locals, var.Name, argsCount));
                    line += " #" + var.Name;
                }
                assemblyText.Add(line);
            }
        }
        int getLevel(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != '\t')
                {
                    return i;
                }
            }
            return 0;
        }
        Variable variableDeclarator(String line)
        {
            Match matchDef = Regex.Match(line, @"^(?<type>(int)|(char)|(str))[ ]+" + variableNameRegex + @"[\s]*$");
            if (!matchDef.Success)
                return null;
            string name = matchDef.Groups["name"].Value;
            string type = matchDef.Groups["type"].Value;
            if (type == "int")
                return new Variable() { Name = name, Size = 4, Type = VarType.integer };
            if (type == "str")
                return new Variable() { Name = name, Size = 4, Type = VarType.str };
            if (type == "char")
                return new Variable() { Name = name, Size = 1, Type = VarType.character };
            return null;
        }
        bool isVariableOperiation(string line)
        {
            Match matchDef = Regex.Match(line, variableNameRegex + @"(=)|(\+=)|(\+\+)");
            return matchDef.Success;
        }
        void variableOperation(string line, Dictionary<string, Variable> locals, int argsCount)
        {
            Match matchDef = Regex.Match(line, variableNameRegex + @"(=)");
            if (matchDef.Success)
            {
                variableAssigment(line, locals, argsCount);
            }
        }
        void variableAssigment(string line, Dictionary<string, Variable> locals, int argsCount)
        {
            Variable var = locals[Regex.Match(line, variableNameRegex).Groups["name"].Value];
            variableExtractor(line.Split('=')[1].Trim(), locals);
            assemblyText.Add("mov %ebx," + buildStackVariable(locals, var.Name, argsCount) + " #" + var.Name);
        }
        void addressGetter(string operations, Dictionary<string, Variable> locals)
        {
            Match match = Regex.Match(operations, variableNameRegex);

            Variable variable = locals[match.Groups["name"].Value];

            assemblyText.Add("lea "+buildStackVariable(locals,variable.Name,0)+",%ebx");
        }
        void integerOperationExtractor(string operations, Dictionary<string, Variable> locals, int argsCount)
        {
            if (Regex.IsMatch(operations, "^-?[0-9]+$"))
            {
                Match match = Regex.Match(operations, "^(?<value>-?[0-9]+)$");
                int value = int.Parse(match.Groups["value"].Value);
                assemblyText.Add("mov $" + value + ",%ebx");
            }
        }
        void stringAssigment(string operations, Dictionary<string, Variable> locals, int argsCount)
        {
            string constString = Regex.Match(operations, "^(?<value>\".*\")$").Groups["value"].Value;
            assemblyData.Add("const_" + constCounter + ": .asciz" + constString);
            assemblyText.Add("mov $const_" + constCounter++ + ",%ebx");
        }
        bool isFunctionCalling(string line)
        {
            return Regex.IsMatch(line, @"^(\^)?" + variableNameRegex + @"[ ]*\(.*\)$");
        }
        void functionCalling(string line, Dictionary<string, Variable> locals, int argsCount)
        {
            Match match = Regex.Match(line, "^" + variableNameRegex + @"[ ]*\((?<args>.*)\)$");
            string functionName;
            if (!match.Success)
            {
                match = Regex.Match(line, @"^(\^)" + variableNameRegex + @"[ ]*\((?<args>.*)\)$");
                functionName = "_" + match.Groups["name"].Value;
            }
            else
            {
                functionName = match.Groups["name"].Value;
            }
            var matchArgs = match.Groups["args"].Value;
            if (matchArgs == "")
            {
                assemblyText.Add("call " + functionName+" #"+line.Trim());
                return;
            }
            string[] arguments = matchArgs.Split(',');
            assemblyText.Add("mov %esp,%edx");
            assemblyText.Add("sub $"+arguments.Length *4+",%edx");
            for (int i = arguments.Length - 1; i >= 0; i--)
            {
                var arg = arguments[i];
                if (arg == "")
                    continue;

                variableExtractor(arg,locals);

                assemblyText.Add("mov %ebx,"+i*4+"(%edx)");
            }
            assemblyText.Add("sub $" + arguments.Length *4+ ",%esp");
            assemblyText.Add("call " + functionName + " #" + line.Trim());
            assemblyText.Add("add $" + arguments.Length * 4 + ",%esp");
        }
        void variableExtractor(string arg, Dictionary<string, Variable> locals)
        {
            bool address = Regex.IsMatch(arg.Trim(), @"[ ]*\^" + variableNameRegex + "$");
            if (address)
            {
                addressGetter(arg.Trim(), locals);
                return;
            }
            if (Regex.IsMatch(arg.Trim(), "^" + variableNameRegex + "$"))
            {
                assemblyText.Add("mov " + buildStackVariable(locals, arg, 0) + ",%ebx" + " #" + arg);
                return;
            }
            if (Regex.IsMatch(arg.Trim(), "^\".*\"$"))
            {
                stringAssigment(arg.Trim(), locals, 0);
                return;
            }

            if (Regex.IsMatch(arg.Trim(), @"(\^)?" + variableNameRegex + @"[ ]*\(.*\)$"))
            {
                functionCalling(arg.Trim(), locals, 0);
                assemblyText.Add("mov %eax,%ebx");
                return;
            }
            if (Regex.IsMatch(arg.Trim(), @"^[\+\-0-9]+$"))
            {
                integerOperationExtractor(arg.Trim(), locals, 0);
                return;
            }
            throw new Exception();
        }
        string buildStackVariable(Dictionary<string, Variable> locals, string name, int argsCount)
        {
            Variable variable = locals[name];
            return ((locals.Keys.Count - 1 - variable.StackPosition) * 4) + "(%esp)";
        }
        public void SaveASM()
        {
            var stream = File.Open(filename + ".s", FileMode.Create);
            TextWriter writer = new StreamWriter(stream);
            writer.WriteLine(".data");
            foreach (var line in assemblyData)
            {
                writer.WriteLine(line);
            }
            writer.WriteLine(".text");
            foreach (var line in assemblyText)
            {
                writer.WriteLine(line);
            }
            writer.Flush();
            stream.Close();
        }

    }
    struct Block
    {
        public int Start;
        public int End;
        public Block(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
