#include "stdafx.h"
#include <iostream>
#include "CppUnitTest.h"
#include "../BehaviorTreesLib/BehaviorTree.h"
#include "../BehaviorTreesLib/BehaviorTreeBuilder.h"
#include <thread>
#include <chrono>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace fluentBehaviorTree;
using namespace std;

namespace Microsoft
{
	namespace VisualStudio
	{
		namespace CppUnitTestFramework
		{
			template<> static std::wstring ToString<EStatus>(const EStatus& t)
			{
				switch (t)
				{
				case EStatus::SUCCESS:
					return L"EStatus::SUCCESS";
					break;
				case EStatus::FAILURE:
					return L"EStatus::FAILURE";
					break;
				case EStatus::RUNNING:
					return L"EStatus::RUNNING";
					break;
				case EStatus::ERROR:
					return L"EStatus::ERROR";
					break;
				default:
					return L"EStatus";
					break;
				}
			}
		}
	}
}

namespace BTTestsCPP
{
	typedef std::function<bool()> BoolLambda;
	typedef std::function<EStatus()> EStatusLambda;

	TEST_CLASS(BTMLTests)
	{
		TEST_METHOD(SingleTreeTest)
		{
			// Way is blocked, window is locked
			ticked = 0;
			isWayBlocked = true;
			isWindowLocked = true;

			Assert::AreEqual(EStatus::FAILURE, RunTree(&singleTree));

			// Way is blocked, window is open
			ticked = 0;
			isWayBlocked = true;
			isWindowLocked = false;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&singleTree));

			// Way is not blocked, door is open
			ticked = 0;
			isWayBlocked = false;
			isDoorLocked = false;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&singleTree));

			// Way is not blocked, door is locked but have key
			ticked = 0;
			isWayBlocked = false;
			isDoorLocked = true;
			haveKey = true;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&singleTree));

			// Way is not blocked, door is locked and doesn't have key
			// Strong enough to break door
			ticked = 0;
			isWayBlocked = false;
			isDoorLocked = true;
			haveKey = false;
			STR = 15;
			DC = 15;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&singleTree));

			// Way is not blocked, door is locked and doesn't have key
			// Too weak to break door. Someone coming
			ticked = 0;
			isWayBlocked = true;
			isDoorLocked = true;
			haveKey = false;
			STR = 10;
			DC = 15;
			someoneCame = true;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&singleTree));

			// Way is not blocked, door is locked and doesn't have key
			// Too weak to break door. No one coming. Window locked
			ticked = 0;
			isWayBlocked = true;
			isDoorLocked = true;
			haveKey = false;
			STR = 10;
			DC = 15;
			someoneCame = false;
			isWindowLocked = true;

			Assert::AreEqual(EStatus::FAILURE, RunTree(&singleTree));
		}

		TEST_METHOD(NestedTreeTest)
		{
			// Way is blocked, window is locked
			ticked = 0;
			isWayBlocked = true;
			isWindowLocked = true;

			Assert::AreEqual(EStatus::FAILURE, RunTree(&nestedTree));

			// Way is blocked, window is open
			ticked = 0;
			isWayBlocked = true;
			isWindowLocked = false;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&nestedTree));

			// Way is not blocked, door is open
			ticked = 0;
			isWayBlocked = false;
			isDoorLocked = false;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&nestedTree));

			// Way is not blocked, door is locked but have key
			ticked = 0;
			isWayBlocked = false;
			isDoorLocked = true;
			haveKey = true;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&nestedTree));

			// Way is not blocked, door is locked and doesn't have key
			// Strong enough to break door
			ticked = 0;
			isWayBlocked = false;
			isDoorLocked = true;
			haveKey = false;
			STR = 15;
			DC = 15;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&nestedTree));

			// Way is not blocked, door is locked and doesn't have key
			// Too weak to break door. Someone coming
			ticked = 0;
			isWayBlocked = true;
			isDoorLocked = true;
			haveKey = false;
			STR = 10;
			DC = 15;
			someoneCame = true;

			Assert::AreEqual(EStatus::SUCCESS, RunTree(&nestedTree));

			// Way is not blocked, door is locked and doesn't have key
			// Too weak to break door. No one coming. Window locked
			ticked = 0;
			isWayBlocked = true;
			isDoorLocked = true;
			haveKey = false;
			STR = 10;
			DC = 15;
			someoneCame = false;
			isWindowLocked = true;

			Assert::AreEqual(EStatus::FAILURE, RunTree(&nestedTree));
		}
	public:
		// Helper
		static bool isWayBlocked;
		static bool isDoorLocked;
		static bool haveKey;
		static int STR;
		static int DC;
		static bool isDoorBroken;
		static bool someoneCame;
		static bool isWindowLocked;
		static int ticked;

		static BoolLambda IsWayBlocked;
		static BoolLambda IsDoorBroken;
		static BoolLambda SomeoneCame;

		static EStatusLambda GoToDoor;
		static EStatusLambda OpenDoor;
		static EStatusLambda UnlockDoor;
		static EStatusLambda BreakDoor;
		static EStatusLambda CloseDoor;
		static EStatusLambda AskToOpenDoor;
		static EStatusLambda GoToWindow;
		static EStatusLambda OpenWindow;
		static EStatusLambda CloseWindow;

		static BehaviorTree tryDoorSequence;
		static BehaviorTree tryWindowSequence;
		static BehaviorTree nestedTree;
		static BehaviorTree singleTree;

	private:

		EStatus RunTree(BehaviorTree* tree)
		{
			EStatus result = EStatus::ERROR;
			do
			{
				result = tree->tick();
			} while (result != EStatus::SUCCESS && result != EStatus::FAILURE);
			return result;
		}
	};

	bool BTMLTests::isWayBlocked = false;
	bool BTMLTests::isDoorLocked = true;
	bool BTMLTests::haveKey = false;
	int  BTMLTests::STR = 10;
	int  BTMLTests::DC = 20;
	bool BTMLTests::isDoorBroken = false;
	bool BTMLTests::someoneCame = false;
	bool BTMLTests::isWindowLocked = true;
	int  BTMLTests::ticked = 0;

	BoolLambda BTMLTests::IsWayBlocked = []() { return BTMLTests::isWayBlocked; };

	EStatusLambda BTMLTests::GoToDoor = []()
	{
		if (BTMLTests::ticked < 5)
		{
			cout << "I'm on my way" << endl;
			BTMLTests::ticked++;
			return EStatus::RUNNING;
		}
		else
		{
			cout << "I'm at the door" << endl;
			return EStatus::SUCCESS;
		}
	};

	EStatusLambda BTMLTests::OpenDoor = []()
	{
		if (BTMLTests::isDoorLocked)
		{
			cout << "Door is locked" << endl;
			return EStatus::FAILURE;
		}
		else
		{
			cout << "Opening door" << endl;
			return EStatus::SUCCESS;
		}
	};

	EStatusLambda BTMLTests::UnlockDoor = []()
	{
		if (!BTMLTests::haveKey)
		{
			cout << "Can't unlock the door" << endl;
			return EStatus::FAILURE;
		}
		else
		{
			cout << "Door unlocked" << endl;
			return EStatus::SUCCESS;
		}
	};

	EStatusLambda BTMLTests::BreakDoor = []()
	{
		if (BTMLTests::DC > BTMLTests::STR)
		{
			cout << "Door is too strong" << endl;
			return EStatus::FAILURE;
		}
		else
		{
			cout << "Breaking door" << endl;
			BTMLTests::isDoorBroken = true;
			return EStatus::SUCCESS;
		}
	};

	BoolLambda BTMLTests::IsDoorBroken = []() { return BTMLTests::isDoorBroken; };

	EStatusLambda BTMLTests::CloseDoor = []()
	{
		if (!BTMLTests::isDoorBroken)
		{
			cout << "Closing door" << endl;
			return EStatus::SUCCESS;
		}
		else
		{
			cout << "Can't close a broken door" << endl;
			return EStatus::FAILURE;
		}
	};

	BoolLambda BTMLTests::SomeoneCame = []()
	{
		if (BTMLTests::someoneCame)
		{
			cout << "Hey, someone's here!" << endl;
			return true;
		}
		else
		{
			cout << "No one seems to be coming" << endl;
			return false;
		}
	};

	EStatusLambda BTMLTests::AskToOpenDoor = []()
	{
		cout << "Person opened the door" << endl;
		return EStatus::SUCCESS;
	};

	EStatusLambda BTMLTests::GoToWindow = []()
	{
		if (BTMLTests::ticked < 9)
		{
			cout << "I'm on my way" << endl;
			ticked++;
			return EStatus::RUNNING;
		}
		else
		{
			cout << "I'm at the window" << endl;
			return EStatus::SUCCESS;
		}
	};

	EStatusLambda BTMLTests::OpenWindow = []()
	{
		if (BTMLTests::isWindowLocked)
		{
			cout << "Window is locked" << endl;
			return EStatus::FAILURE;
		}
		else
		{
			cout << "Opening window" << endl;
			return EStatus::SUCCESS;
		}
	};

	EStatusLambda BTMLTests::CloseWindow = []()
	{
		cout << "Closing window" << endl;
		return EStatus::SUCCESS;
	};

	BehaviorTree BTMLTests::tryDoorSequence = BehaviorTreeBuilder("Open door")
		.Sequence("Try door")
		.Not("Way is not blocked")
		.If("Is way blocked?", IsWayBlocked)
		.Do("Go to door", GoToDoor)
		.Selector("Open door selector")
		.Do("Open door", OpenDoor)
		.Do("Unlock door", UnlockDoor)
		.Do("Break door down", BreakDoor)
		.Sequence("Check if anyone comes")
		.Wait("Wait for people to come", 5000)
		.If("Someone came", SomeoneCame)
		.Do("Ask them to open door", AskToOpenDoor)
		.End()
		.End()
		.Ignore("Try to close door")
		.Do("Close door", CloseDoor)
		.End()
		.End();

	BehaviorTree BTMLTests::tryWindowSequence = BehaviorTreeBuilder("OpenWindow")
		.Sequence("Try window")
		.Do("Go to window", GoToWindow)
		.Do("Open window", OpenWindow)
		.Do("Close window", CloseWindow)
		.End()
		.End();

	BehaviorTree BTMLTests::nestedTree = BehaviorTreeBuilder("Nested tree")
		.Selector("Find an entrance")
		.Do("Try door", tryDoorSequence.getRoot())
		.Do("Try window", tryWindowSequence.getRoot())
		.End()
		.End();

	BehaviorTree BTMLTests::singleTree = BehaviorTreeBuilder("Single tree")
		.Selector("Find an entrance")
		.Sequence("Try door")
		.Not("Way is not blocked")
		.If("Is way blocked?", IsWayBlocked)
		.Do("Go to door", GoToDoor)
		.Selector("Open door selector")
		.Do("Open door", OpenDoor)
		.Do("Unlock door", UnlockDoor)
		.Do("Break door down", BreakDoor)
		.Sequence("Check if anyone comes")
		.Wait("Wait for people to come", 5000)
		.If("Someone came", SomeoneCame)
		.Do("Ask them to open door", AskToOpenDoor)
		.End()
		.End()
		.Ignore("Try to close door")
		.Do("Close door", CloseDoor)
		.End()
		.Sequence("Try window")
		.Do("Go to window", GoToWindow)
		.Do("Open window", OpenWindow)
		.Do("Close window", CloseWindow)
		.End()
		.End()
		.End();
}