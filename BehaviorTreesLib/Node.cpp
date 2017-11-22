#include "Node.h"

namespace fluentBehaviorTree
{
	// 
	EStatus Node::tick()
	{
		enter();

		if (!mIsOpen) open();

		EStatus status = tickNode();

		exit();
		if (status == EStatus::RUNNING) return status;

		close();
		return status;
	}

	// Enter node and prepare for execution
	void Node::enter()
	{
		//
	}

	// Open node only if node has not been opened before
	void Node::open()
	{
		mIsOpen = true;

		mResult = EStatus::RUNNING;
	}

	// Exit node after every tick
	void Node::exit()
	{
		//
	}

	// Close node to ensure we don't go through it again
	void Node::close()
	{
		mIsOpen = false;
	}
}