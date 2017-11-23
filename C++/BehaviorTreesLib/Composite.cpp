#include "Composite.h"

namespace fluentBehaviorTree
{
	void Composite::addChild(Node & n)
	{
		mChildren.push_back(&n);
	}

	void Composite::open()
	{
		Node::open();
		for each(auto child in mChildren)
		{
			child->setResult(EStatus::RUNNING);
		}
	}
}
