#ifndef BT_COMPOSITE_H
#define BT_COMPOSITE_H

#include "Branch.h"
#include <vector>

namespace fluentBehaviorTree
{
	class Composite : public Branch
	{
	public:
		std::vector<Node*> mChildren;

		void addChild(Node& n) override;

	protected:
		void open() override;
	};
}

#endif
