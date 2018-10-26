#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>

using namespace std;



// Split string by character
vector<string> separateStringBy(const string& s, const char& d)
{
	vector<string> parts;
	int i = s.find_first_of(d);
	parts.push_back(s.substr(0, i));
	parts.push_back(s.substr(i + 1));
	return parts;
}

// Trim the string
string trimString(const string& s)
{
	string str = s;
	const string whitespace = " \t";
	const auto begin = str.find_first_not_of(whitespace);
	if (begin == std::string::npos)
		return "";

	const auto end = str.find_last_not_of(whitespace);
	const auto range = end - begin + 1;
	return str.substr(begin, range);
}

// Check if string is a number
bool isNumber(const std::string& s)
{
	return !s.empty() && s.find_first_not_of("0123456789") == std::string::npos;
}

// Count amount of tabs in specified string
int countTabs(string s)
{
	int num = 0;
	while (s[0] == '\t')
	{
		s = s.substr(1);
		num++;
	}
	return num;
}

// Produce a lambda of the specified type in the specified language
string getLambda(string type, bool needsCPP)
{
	string enumReturn = needsCPP ? "EStatus::ERROR" : "Status.ERROR";
	string returnValue = type == "action" ? enumReturn : "false";
	string lambdaStart = needsCPP ? "[]()" : "() =>";

	return lambdaStart + " { return " + returnValue + "; }";
}

// 
int main(int argc, char* argv[])
{
	if (argc == 0)
	{
		cout << "Usage:\n" <<
			"btmlparsercpp <C# | C++> <input path> [<output path>]" << endl;
	}
	else if (argc > 0)
	{
		char* outputFile = ".\\output.txt";
		char* language = argv[1];
		char* filename = argv[2];
		if (argv[3] != NULL)
		{
			outputFile = argv[3];
		}
		ifstream ifile(filename);
		if (ifile)
		{
			bool convertingToCPP = (language == "C++" || language == "c++");
			if (!convertingToCPP && language != "c#" && language != "c#")
			{
				cout << "No language specified: " << language << endl; return 0;
			}
			const char separator = ' ';
			string line = "";

			int currentLevel = 0;
			int levelsOpen = 0;
			int tabNum = 0;
			string type = "";
			vector<string> parts;

			string output = "BehaviorTree tree = " + string(convertingToCPP ? "" : "new") + " BehaviorTreeBuilder(\"\")";

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
					// TODO: Needs to open file and do this step until tree is completed
					output += "\n.Do(\"" + parts[1] + "\")";
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
			output += "\n.End();";

			std::remove(outputFile);
			ofstream ofile(outputFile);
			ofile.clear();
			if (ofile.is_open())
			{
				ofile << output << std::endl;
			}
			ofile.close();
		}
	}
	else
	{
		cout << "Unable to find file" << endl;
	}
	return 0;
}
