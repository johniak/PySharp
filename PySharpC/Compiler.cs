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
        int ifCounter = 0;
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
                if (Regex.IsMatch(lines[currentLine].Trim(), @"^[\s]*#"))
                {
                    currentLine++;
                    continue;
                }
                Variable var = variableDeclarator(lines[currentLine].Trim());
                if (var != null)
                {
                    var.StackPosition = locals.Count;
                    localLocals.Add(var);
                    locals.Add(var.Name, var);
                    assemblyText.Add("sub $4,%esp #" + var.Name);
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
                if (isIfStatment(lines[currentLine]))
                {
                    Block newBlock = findBlock(level + 1, currentLine + 1);
                    ifStatmentCompiler(newBlock, locals, level + 1);
                }
                if (Regex.IsMatch(lines[currentLine].Trim(), @"^ret[ ]+"))
                {
                    variableExtractor(lines[currentLine].Trim().Substring(3), locals);
                    assemblyText.Add("mov %ebx,%eax");
                    assemblyText.Add("add $" + localLocals.Count * 4 + ",%esp");
                    assemblyText.Add("mov %ebp,%esp");
                    assemblyText.Add("pop %ebx");
                    assemblyText.Add("pop %ebp");
                    assemblyText.Add("ret");
                    currentLine++;
                }
                currentLine++;

            }
            assemblyText.Add("add $" + localLocals.Count * 4 + ",%esp");

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
            Match matchDef = Regex.Match(line.Trim(), "^" + variableNameRegex + @"(=)|(\+=)|(\+\+)");
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

            assemblyText.Add("lea " + buildStackVariable(locals, variable.Name, 0) + ",%ebx");
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
                assemblyText.Add("call " + functionName + " #" + line.Trim());
                return;
            }
            string[] arguments = matchArgs.Split(',');
            assemblyText.Add("mov %esp,%edx");
            assemblyText.Add("sub $" + arguments.Length * 4 + ",%edx");
            for (int i = arguments.Length - 1; i >= 0; i--)
            {
                var arg = arguments[i];
                if (arg == "")
                    continue;

                variableExtractor(arg, locals);

                assemblyText.Add("mov %ebx," + i * 4 + "(%edx)");
            }
            assemblyText.Add("sub $" + arguments.Length * 4 + ",%esp");
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
        bool isIfStatment(string line)
        {
            return Regex.IsMatch(line.Trim(), @"^if[ ]*\([ ]*.*[ ]*\)[ ]*:");
        }
        void ifStatmentCompiler(Block block, Dictionary<string, Variable> locals, int level)
        {
            var command = Regex.Match(lines[block.Start - 1].Trim(), @"^if[ ]*\((?<command>[ ]*.*[ ]*)\)[ ]*:").Groups["command"].Value.Trim();
            int ifEndNumber = ifCounter++;
            int ifStartNumber = ifCounter++;
            ifLogicCompiler(command, locals);
            smallLogicCompiler(command, locals, false, ifStartNumber, ifEndNumber);
            assemblyText.Add("if" + ifStartNumber + ":");
            blockCompilator(block, locals, level, 0);
            assemblyText.Add("if" + ifEndNumber + ":");
        }
        void smallLogicCompiler(string command, Dictionary<string, Variable> locals, bool endWithOr, int ifStartNumber, int ifEndNumber)
        {
            var logicOrExpresions =orSplitter(command);
            
            
            for (int i = 0; i < logicOrExpresions.Length; i++)
            {
                var ors = logicOrExpresions[i];
                var logicAndExpresions = andSplitter(ors);
                var end = ifEndNumber;
                if (i < logicOrExpresions.Length - 1)
                {
                    end = ifCounter++;
                }
                for (int j = 0; j < logicAndExpresions.Length - 1; j++)
                {
                    var ands = logicAndExpresions[j].Trim();
                    //Match match = Regex.Match(ands, @"\((?<expr>.*)\)");
                    //if (match.Success)
                    //{
                    //    smallLogicCompiler(match.Groups["expr"].Value,locals,false,ifStartNumber,ife)
                    //}
                    //else
                    ifAndComparrer(ands, locals, end);
                }
                if (i < logicOrExpresions.Length - 1 || endWithOr)
                {
                    var ands = logicAndExpresions[logicAndExpresions.Length - 1];
                    ifOrComparrer(ands, locals, ifStartNumber);
                }
                else
                {
                    var ands = logicAndExpresions[logicAndExpresions.Length - 1];
                    ifAndComparrer(ands, locals, ifEndNumber);
                }

                if (i < logicOrExpresions.Length - 1)
                {
                    assemblyText.Add("if" + end + ":");
                }
            }
        }
        string[] orSplitter(string command)
        {
            List<string> ors= new List<string>();
            int lastOr=0;
            int level = 0;
            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == '(')
                    level++;
                if (command[i] == ')')
                    level--;
                if (command[i] == '|' && i < command.Length - 1 && command[i + 1] == '|'&&level==0)
                {
                    ors.Add(command.Substring(lastOr,i-lastOr));
                    lastOr = i + 2;
                }
            }
            ors.Add(command.Substring(lastOr, command.Length - lastOr));
            return ors.ToArray();
        }
        string[] andSplitter(string command)
        {
            List<string> ands = new List<string>();
            int lastAnd = 0;
            int level = 0;
            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == '(')
                    level++;
                if (command[i] == ')')
                    level--;
                if (command[i] == '&' && i < command.Length - 1 && command[i + 1] == '&' && level == 0)
                {
                    ands.Add(command.Substring(lastAnd, i - lastAnd));
                    lastAnd = i + 2;
                }
            }
            ands.Add(command.Substring(lastAnd, command.Length - lastAnd));
            return ands.ToArray();
        }
        void ifAndComparrer(string command, Dictionary<string, Variable> locals, int ifEndNumber)
        {
            var vars = Regex.Split(command, "==|<=|>=|!=|>|<");
            var type = Regex.Match(command, "(?<type>==|<=|>=|!=|>|<)").Groups["type"].Value;
            variableExtractor(vars[0], locals);
            Variable variable = new Variable();
            variable.StackPosition = locals.Count;
            locals.Add("__TMP", variable);
            assemblyText.Add("sub $4,%esp #__TMP");
            assemblyText.Add("mov %ebx," + buildStackVariable(locals, "__TMP", 0));
            variableExtractor(vars[1], locals);
            assemblyText.Add("mov " + buildStackVariable(locals, "__TMP", 0) + ",%eax");
            assemblyText.Add("add $4,%esp #__TMP");
            locals.Remove("__TMP");
            assemblyText.Add("cmp %ebx,%eax");
            if (type == "==")
            {
                assemblyText.Add("jne if" + ifEndNumber);
            }
            else if (type == "<=")
            {
                assemblyText.Add("jg if" + ifEndNumber);
            }
            else if (type == ">=")
            {
                assemblyText.Add("jl if" + ifEndNumber);
            }
            else if (type == "!=")
            {
                assemblyText.Add("je if" + ifEndNumber);
            }
            else if (type == ">")
            {
                assemblyText.Add("jle if" + ifEndNumber);
            }
            else if (type == "<")
            {
                assemblyText.Add("jge if" + ifEndNumber);
            }
        }
        void ifOrComparrer(string command, Dictionary<string, Variable> locals, int ifStartNumber)
        {
            var vars = Regex.Split(command, "==|<=|>=|!=|>|<");
            var type = Regex.Match(command, "(?<type>==|<=|>=|!=|>|<)").Groups["type"].Value;
            variableExtractor(vars[0], locals);
            Variable variable = new Variable();
            variable.StackPosition = locals.Count;
            locals.Add("__TMP", variable);
            assemblyText.Add("sub $4,%esp #__TMP");
            assemblyText.Add("mov %ebx," + buildStackVariable(locals, "__TMP", 0));
            variableExtractor(vars[1], locals);
            assemblyText.Add("mov " + buildStackVariable(locals, "__TMP", 0) + ",%eax");
            assemblyText.Add("add $4,%esp #__TMP");
            locals.Remove("__TMP");
            assemblyText.Add("cmp %ebx,%eax");
            if (type == "==")
            {
                assemblyText.Add("je if" + ifStartNumber);
            }
            else if (type == "<=")
            {
                assemblyText.Add("jle if" + ifStartNumber);
            }
            else if (type == ">=")
            {
                assemblyText.Add("gle if" + ifStartNumber);
            }
            else if (type == "!=")
            {
                assemblyText.Add("jne if" + ifStartNumber);
            }
            else if (type == ">")
            {
                assemblyText.Add("jg if" + ifStartNumber);
            }
            else if (type == "<")
            {
                assemblyText.Add("jl if" + ifStartNumber);
            }
        }

        void ifLogicCompiler(string command, Dictionary<string, Variable> locals)
        {

            command = command.Substring(2).Trim();
            List<String> brackets = new List<String>();
            Match match = Regex.Match(command, @"\(.*\)");


            while (match.Success)
            {
                brackets.Add(match.Groups[0].Value.Substring(1, match.Groups[0].Value.Length - 2));
                match = Regex.Match(brackets[brackets.Count - 1], @"\(.*\)");
            }
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
