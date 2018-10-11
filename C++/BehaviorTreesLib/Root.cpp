#include "Root.h"

namespace fluentBehaviorTree
{
	void Root::addChild(Node & n)
	{
		mChild = &n;
	}
	Root::Root()
	{
		this->setName("Root");
	}

	Root::~Root()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}

	// Ticks only child and returns
	EStatus Root::tickNode()
	{
		this->setResult(mChild->tick());

		return this->getResult();
	}
}