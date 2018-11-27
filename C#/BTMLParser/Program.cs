using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BTMLParser
{
    class Program
    {
        /// <summary>
        /// Produce a lambda of the specified type in the specified language
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static string GetLambda(string type)
        {
            return "() => { return " + (type == "action" ? "Status.ERROR" : "false") + ";}";
        }

        // Count amount of tabs in specified string
        int CountTabs(string s)
        {
            const string tab = "\t";
            const string tabSpaces = "    ";
            int num = 0;
            while (s.StartsWith(tab) || s.StartsWith(tabSpaces))
            {
                if (s.StartsWith(tab))
                {
                    s = s.Substring(1);
                    num++;
                }
                else
                {
                    s = s.Substring(4);
                    num += 4;
                }
            }
            return num;
        }

        static char[] separatorArray = { ' ' };
        // Convert the map into the text
        string parse(string filename, string language, bool isBase = false)
        {
            string output = "";

            File ifile(filename.c_str());
            if (ifile.good())
            {
                bool convertingToCPP = (language == "C++" || language == "c++");
                if (!convertingToCPP && language != "C#" && language != "c#")
                {
                    cout << "No language specified: " << language << endl;
                    return 0;
                }
                const char separator = ' ';
                string line = "";

                int currentLevel = 0;
                int levelsOpen = 0;
                int tabNum = 0;
                string type = "";
                vector<string> parts;

                if (isBase) output = "BehaviorTree tree = " + string(convertingToCPP ? "" : "new") + " BehaviorTreeBuilder(\"\")";

                while (getline(ifile, line))
                {
                    tabNum = countTabs(line);
                    if (0 < tabNum && tabNum <= currentLevel)
                    {
                        output += "\n.End()";
                        levelsOpen--;
                    }

                    istringstream iss(trimString(line));
                    parts = separateStringBy(iss.str(), separator);
                    type = parts[0];

                    if (type == "!")
                    {
                        output += "\n.Do(\"" + parts[1] + "\", " + getLambda("action", convertingToCPP) + ")";
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
                        output += "\n.If(\"" + parts[1] + "\"," + getLambda("condition", convertingToCPP) + ")";
                    }
                    else if (type == "¬")
                    {
                        output += "\n.Not(\"" + parts[1] + "\")";
                    }
                    else if (isNumber(type))
                    {
                        int result = stoi(type);
                        output += "\n.Repeat(\"" + parts[1] + (result > 0 ? "\", " + type : "") + ")";
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
                    else if (type == "#") // Subtree
                    {
                        string subtreeFilename = filename.substr(0, filename.find_last_of('\\') + 1) + parts[1];
                        output += parse(subtreeFilename, language);
                    }
                    else
                    {
                        // Do nothing
                    }
                }
                ifile.close();
                while (levelsOpen > 0)
                {
                    output += "\n.End()";
                    levelsOpen--;
                }
                if (isBase) output += "\n.End();";
            }

            return output;
        }

        // 
        int Main(string[] args)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("btmlparsercpp.exe <C#|C++> <input path> [<output path>]");
            }
            else if (args.Length > 1)
            {
                string language = args[1];
                string filename = args[2];

                string output = parse(filename, language, true);

                string outputFile;
                // If an output file is provided, output to that file
                if (args.Length > 3)
                {
                    outputFile = args[3];

                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }
                    File.WriteAllText(outputFile, output);
                }
                // Otherwise, output the text to the console
                else
                {
                    Console.WriteLine(output);
                }
            }
            else
            {
                Console.WriteLine("Unable to find file");
            }
            return 0;
        }
    }
}
