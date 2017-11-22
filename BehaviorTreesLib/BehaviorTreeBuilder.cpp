#include "BehaviorTreeBuilder.h"

#include "Action.h"
#include "Condition.h"
#include "Inverter.h"
#include "Repeater.h"
#include "RepeatUntilFail.h"
#include "Succeeder.h"
#include "Timer.h"
#include "Selector.h"
#include "Sequence.h"

namespace fluentBehaviorTree
{
	BehaviorTreeBuilder::BehaviorTreeBuilder(std::string name)
	{
		this->setName(name);
	}

	/*********************/
	/******* LEAF ********/
	/*********************/

	// Add an action to the tree
	BehaviorTreeBuilder BehaviorTreeBuilder::Do(std::string name, EStatus(*f)())
	{
		Action currentNode(name, f);
		

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}
		Decorator* typeCheck;
		while (mParentNodes.size() > 1 && typeCheck)
		{
			mParentNodes.pop();              // Remove decorators from parentNodes until you reach next Composite
			typeCheck = dynamic_cast<Decorator*>(mParentNodes.top());
		}

		return *this;
	}

	// Add condition
	BehaviorTreeBuilder BehaviorTreeBuilder::If(std::string name, bool(*f)())
	{
		Condition currentNode(name, f);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		Decorator* typeCheck;
		while (mParentNodes.size() > 1 && typeCheck)
		{
			mParentNodes.pop();              // Remove decorators from parentNodes until you reach next Composite
			typeCheck = dynamic_cast<Decorator*>(mParentNodes.top());
		}

		return *this;
	}


	/*********************/
	/***** DECORATOR *****/
	/*********************/

	// Negate child
	BehaviorTreeBuilder BehaviorTreeBuilder::Not(std::string name)
	{
		Inverter currentNode(name);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}

	// Repeatedly tick child n times. If n == 0, tick forever
	BehaviorTreeBuilder BehaviorTreeBuilder::Repeat(std::string name, int n = 0)
	{
		Repeater currentNode(name, n);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}

	// Repeatedly tick child until FAILURE is returned
	BehaviorTreeBuilder BehaviorTreeBuilder::RepeatUntilFail(std::string name)
	{
		fluentBehaviorTree::RepeatUntilFail currentNode(name);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}

	// Tick child and return SUCCESS
	BehaviorTreeBuilder BehaviorTreeBuilder::Ignore(std::string name)
	{
		Succeeder currentNode(name);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}

	// Wait ms seconds and execute node
	BehaviorTreeBuilder BehaviorTreeBuilder::Wait(std::string name, size_t ms)
	{
		Timer currentNode(name, ms);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}

	/*********************/
	/***** COMPOSITE *****/
	/*********************/

	// Add a selector
	BehaviorTreeBuilder BehaviorTreeBuilder::Selector(std::string name)
	{
		fluentBehaviorTree::Selector currentNode(name);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}


	// Add a sequence
	BehaviorTreeBuilder BehaviorTreeBuilder::Sequence(std::string name)
	{
		fluentBehaviorTree::Sequence currentNode(name);

		if (mParentNodes.size() > 0)
		{
			mParentNodes.top()->addChild(currentNode);
		}

		mParentNodes.push(&currentNode);

		return *this;
	}

	// Close children group
	BehaviorTreeBuilder BehaviorTreeBuilder::End()
	{
		if (mParentNodes.size() > 1)
		{
			mParentNodes.pop();
		}
		return *this;
	}
}