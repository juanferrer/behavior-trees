#include "Node.h"

namespace fluentBehaviorTree
{
	// 
	EStatus Node::Tick()
	{
		Enter();

		if (!mIsOpen) Open();

		EStatus status = tick();

		Exit();
		if (status == RUNNING) return status;

		Close();
		return status;
	}

	// Enter node and prepare for execution
	void Enter()
	{
		//
	}

	/// <summary>
	/// Open node only if node has not been opened before
	/// </summary>
	void Node::Open()
	{
		mIsOpen = true;

		mResult = RUNNING;
	}

	// Exit node after every tick
	void Exit()
	{
		//
	}

	// Close node to ensure we don't go through it again
	void Node::Close()
	{
		mIsOpen = false;
	}
}