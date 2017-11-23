#include "Decorator.h"

namespace fluentBehaviorTree
{
	void Decorator::addChild(Node & n)
	{
		mChild = &n;
	}
}