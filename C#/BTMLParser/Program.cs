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
        static char[] separatorArray = { ' ' };
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var filename = args[0];
                if (System.IO.File.Exists(filename))
                {
                    string[] lines = System.IO.File.ReadAllLines(filename);
                    int currentLevel = 0;
                    int levelsOpen = 0;
                    int tabNum = 0;
                    string s = "";
                    string type = "";
                    string output = "BehaviorTree tree = new BehaviorTreeBuilder(\"\")";
                    string[] parts;
                    foreach (var line in lines)
                    {
                        tabNum = line.Count(ch => ch == '\t');
                        if (0 < tabNum && tabNum <= currentLevel)
                        {
                            output += "\n.End()";
                            levelsOpen--;
                        }
                        s = line.Substring(tabNum);
                        parts = s.Split(separatorArray, 2);
                        type = parts[0];
                        if (type == "#")
                        {
                            output += "\n.Do(\"" + parts[1] + "\", () => {})";
                        }
                        else if (type == "&")
                        {
                            output += "\n.Sequence(\"" + parts[1] + "\")";
                            currentLevel = tabNum;
                            levelsOpen++;
                        }
                        else if (type == "|")
                        {
                            output += "\n.Selector(\"" + parts[1] + "\")";
                            currentLevel = tabNum;
                            levelsOpen++;
                        }
                        else if (type == "?")
                        {
                            output += "\n.If(\"" + parts[1] + "\", () => {})";
                        }
                        else if (type == "!")
                        {
                            output += "\n.Not(\"" + parts[1] + "\")";
                        }
                        else if (Regex.IsMatch(type, @"\d"))
                        {
                            int result;
                            if (int.TryParse(type, out result))
                            {
                                output += "\n.Repeat(\"" + parts[1] + (result > 0 ? "\", " + result.ToString() : "") + ")";
                            }
                        }
                        else if (type == "*")
                        {
                            output += "\n.RepeatUntilFail(\"" + parts[1] + "\")";
                        }
                        else if (type == "^")
                        {
                            output += "\n.Ignore(\"" + parts[1] + "\")";
                        }
                        else if (type == "\"")
                        {
                            output += "\n.Wait(\"" + parts[1] + "\", 0)";
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

                    System.IO.File.AppendAllText("output.txt", output);
                }
                else
                {
                    Console.WriteLine("Unable to find file");
                }
            }
        }
    }
}
