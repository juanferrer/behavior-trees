#include <iostream>
#include "../BehaviorTreesLib/BehaviorTree.h"
#include "../BehaviorTreesLib/BehaviorTreeBuilder.h"

#include <stdlib.h>
#include <crtdbg.h>
#define _CRTDBG_MAP_ALLOC

using namespace std;
using namespace fluentBehaviorTree;

static bool isWayBlocked = false;
static bool isDoorLocked = true;
static bool haveKey = false;
static int STR = 13;
static int DC = 20;
static bool isDoorBroken = false;
static bool someoneCame = false;
static bool isWindowLocked = false;
static int ticked = 0;

static bool IsWayBlocked()
{
	return isWayBlocked;
}

static EStatus GoToDoor()
{
	if (ticked < 5)
	{
		cout << "I'm on my way" << endl;
		ticked++;
		return EStatus::RUNNING;
	}
	else
	{
		cout << "I'm at the door" << endl;
		return EStatus::SUCCESS;
	}
}

static EStatus OpenDoor()
{
	if (isDoorLocked)
	{
		cout << "Door is locked" << endl;
		return EStatus::FAILURE;
	}
	else
	{
		cout << "Opening door" << endl;
		return EStatus::SUCCESS;
	}
}

static EStatus UnlockDoor()
{
	if (!haveKey)
	{
		cout << "Can't unlock the door" << endl;
		return EStatus::FAILURE;
	}
	else
	{
		cout << "Door unlocked" << endl;
		return EStatus::SUCCESS;
	}
}

static EStatus BreakDoor()
{
	if (DC > STR)
	{
		cout << "Door is too strong" << endl;
		return EStatus::FAILURE;
	}
	else
	{
		cout << "Breaking door" << endl;
		isDoorBroken = true;
		return EStatus::SUCCESS;
	}
}

static bool IsDoorBroken()
{
	return isDoorBroken;
}

static EStatus CloseDoor()
{
	if (!isDoorBroken)
	{
		cout << "Closing door" << endl;
		return EStatus::SUCCESS;
	}
	else
	{
		cout << "Can't close a broken door" << endl;
		return EStatus::FAILURE;
	}
}

static bool SomeoneCame()
{
	if (someoneCame)
	{
		cout << "Hey, someone's here!" << endl;
		return true;
	}
	else
	{
		cout << "No one seems to be coming" << endl;
		return false;
	}
}

static EStatus AskToOpenDoor()
{
	cout << "Person opened the door" << endl;
	return EStatus::SUCCESS;
}

static EStatus GoToWindow()
{
	if (isWayBlocked)
	{
		cout << "The way is blocked" << endl;
		return EStatus::FAILURE;
	}
	else if (ticked < 9)
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
}

static EStatus OpenWindow()
{
	if (isWindowLocked)
	{
		cout << "Window is locked" << endl;
		return EStatus::FAILURE;
	}
	else
	{
		cout << "Opening window" << endl;
		return EStatus::SUCCESS;
	}
}

static EStatus CloseWindow()
{
	cout << "Closing window" << endl;
	return EStatus::SUCCESS;
}

void function()
{
	BehaviorTree openDoor = BehaviorTreeBuilder("Open door")
			.Do("Try to open door", []() { return EStatus::FAILURE; })
		.End();

	BehaviorTree tree = BehaviorTreeBuilder("Enter room")
		.RepeatUntilFail("Base loop")
			.Selector("Find an entrance")
				.Sequence("Try door")
					.Not("Way is not blocked")
						.If("Is way blocked?", IsWayBlocked)
					.Do("Go to door", GoToDoor)
					.Selector("Open door selector")
						.Do("Open door", OpenDoor)
						.Do("Unlock door", UnlockDoor)
						.Do("Break door down", BreakDoor)
				.End()
			.Ignore("Try to close door")
				.Do("Close door", CloseDoor)
			.End()
			.Sequence("Check if anyone comes")
				.Wait("Wait for people to come", 5000)
					.If("Someone came", SomeoneCame)
				.Do("Ask them to open door", AskToOpenDoor)
			.End()
			.Sequence("Try window")
				.Do("Go to window", GoToWindow)
				.Do("Open window", OpenWindow)
				.Do("Close window", CloseWindow)
			.End()
		.End();

	tree.tick();
}

int main()
{
	function();
	_CrtDumpMemoryLeaks();
	return 0;
}