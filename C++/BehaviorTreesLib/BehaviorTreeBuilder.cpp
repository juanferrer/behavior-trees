#include "BehaviorTreeBuilder.h"

#include "Root.h"
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
		auto currentNode = new Root();
		this->setName(name);

		mParentNodes.push(currentNode);
	}

	/*********************/
	/******* LEAF ********/
	/*********************/

	// Add an action to the tree
	BehaviorTreeBuilder BehaviorTreeBuilder::Do(std::string name, EStatus(*f)())
	{
		//Action currentNode(name, f);
		auto currentNode = new Action(name, f);
		

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}
		else throw std::exception("No parent node on Do");
		Decorator* typeCheck = dynamic_cast<Decorator*>(mParentNodes.top());
		while (mParentNodes.size() > 1 && typeCheck)
		{
			mParentNodes.pop();              // Remove decorators from parentNodes until you reach next Composite
			typeCheck = dynamic_cast<Decorator*>(mParentNodes.top());
		}

		return *this;
	}

	BehaviorTreeBuilder BehaviorTreeBuilder::Do(std::string name, BehaviorTree *tree)
	{
		return *this;
	}

	// Add condition
	BehaviorTreeBuilder BehaviorTreeBuilder::If(std::string name, bool(*f)())
	{
		//Condition currentNode(name, f);
		Condition* currentNode = new Condition(name, f);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		Decorator* typeCheck = dynamic_cast<Decorator*>(mParentNodes.top());
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
		//Inverter currentNode(name);
		auto currentNode = new Inverter(name);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

		return *this;
	}

	// Repeatedly tick child n times. If n == 0, tick forever
	BehaviorTreeBuilder BehaviorTreeBuilder::Repeat(std::string name, int n)
	{
		//Repeater currentNode(name, n);
		auto currentNode = new Repeater(name, n);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

		return *this;
	}

	// Repeatedly tick child until FAILURE is returned
	BehaviorTreeBuilder BehaviorTreeBuilder::RepeatUntilFail(std::string name)
	{
		//fluentBehaviorTree::RepeatUntilFail currentNode(name);
		auto currentNode = new fluentBehaviorTree::RepeatUntilFail(name);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

		return *this;
	}

	// Tick child and return SUCCESS
	BehaviorTreeBuilder BehaviorTreeBuilder::Ignore(std::string name)
	{
		//Succeeder currentNode(name);
		auto currentNode = new Succeeder(name);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

		return *this;
	}

	// Wait ms seconds and execute node
	BehaviorTreeBuilder BehaviorTreeBuilder::Wait(std::string name, size_t ms)
	{
		//Timer currentNode(name, ms);
		auto currentNode = new Timer(name, ms);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

		return *this;
	}

	/*********************/
	/***** COMPOSITE *****/
	/*********************/

	// Add a selector
	BehaviorTreeBuilder BehaviorTreeBuilder::Selector(std::string name)
	{
		//fluentBehaviorTree::Selector currentNode(name);
		auto currentNode = new fluentBehaviorTree::Selector(name);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

		return *this;
	}


	// Add a sequence
	BehaviorTreeBuilder BehaviorTreeBuilder::Sequence(std::string name)
	{
		//fluentBehaviorTree::Sequence currentNode(name);
		auto currentNode = new fluentBehaviorTree::Sequence(name);

		if (!mParentNodes.empty())
		{
			mParentNodes.top()->addChild(*currentNode);
		}

		mParentNodes.push(currentNode);

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