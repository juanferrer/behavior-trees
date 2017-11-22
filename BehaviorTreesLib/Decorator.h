#ifndef BT_DECORATOR_H
#define BT_DECORATOR_H

#include "Branch.h"

namespace fluentBehaviorTree
{
	class Decorator : public Branch
	{
	public:
		Node* mChild;
		void addChild(Node& n) override;
	};
}

#endif
