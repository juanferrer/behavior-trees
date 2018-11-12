#ifndef BT_CONDITION_H
#define BT_CONDITION_H

#include "Leaf.h"

namespace fluentBehaviorTree
{
	class Condition : public Leaf
	{
	private:
		Node* copy() override;
		// Function to check
		bool(*mCondition)();

	public:
		Condition(std::string name, bool(*f)());

	protected:
		EStatus tickNode() override;
	};
}

#endif