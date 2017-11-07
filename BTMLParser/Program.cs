using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BTMLParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var filename = args[0];
                string[] lines = System.IO.File.ReadAllLines(filename);
                int currentLevel = 0;
                int levelsOpen = 0;
                int tabNum = 0;
                string s = "";
                string type = "";
                string output = "BehaviorTree tree = new BehaviorTreeBuilder(\"Enter room\")";
                foreach (var line in lines)
                {
                    tabNum = line.Count(ch => ch == '\t');
                    if (0 < tabNum && tabNum <= currentLevel)
                    {
                        output += "\n.End()";
                        levelsOpen--;
                    }
                    s = line.Substring(tabNum);
                    type = s[0].ToString();
                    if (type == "#")
                    {
                        output += "\n.Do(\"" + s.Substring(2) + "\", () => {})";
                    }
                    else if (type == "&")
                    {
                        output += "\n.Sequence(\"" + s.Substring(2) + "\")";
                        currentLevel = tabNum;
                        levelsOpen++;
                    }
                    else if (type == "|")
                    {
                        output += "\n.Selector(\"" + s.Substring(2) + "\")";
                        currentLevel = tabNum;
                        levelsOpen++;
                    }
                    else if (type == "?")
                    {
                        output += "\n.If(\"" + s.Substring(2) + "\", () => {})";
                    }
                    else if (type == "!")
                    {
                        output += "\n.Not(\"" + s.Substring(2) + "\")";
                    }
                    else if (Regex.IsMatch(type, @"\d"))
                    {
                        int result;
                        if (int.TryParse(type, out result))
                        {
                            output += "\n.Repeat(\"" + s.Substring(2) + (result > 0 ? "\", " + result.ToString() : "") + ")";
                        }
                    }
                    else if (type == "*")
                    {
                        output += "\n.RepeatUntilFail(\"" + s.Substring(2) + "\")";
                    }
                    else if (type == "^")
                    {
                        output += "\n.Ignore(\"" + s.Substring(2) + "\")";
                    }
                    else
                    {
                        // Do nothing
                    }
                }
                while (levelsOpen > 0)
                {
                    output += "\n.End()";
                    levelsOpen--;
                }
                output += ";";

                System.IO.File.AppendAllText("output.cs", output);
            }
        }
    }
}
