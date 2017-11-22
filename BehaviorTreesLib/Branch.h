#ifndef BT_BRANCH_H
#define BT_BRANCH_H

#include "Node.h"

namespace fluentBehaviorTree
{
	class Branch : public Node
	{
	public:
		virtual void addChild(Node& n) = 0;
	};
}

#endif
