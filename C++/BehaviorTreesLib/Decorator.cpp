#include "Decorator.h"

namespace fluentBehaviorTree
{
	void Decorator::addChild(Node & n)
	{
		mChild = &n;
	}

	Decorator::~Decorator()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}
}