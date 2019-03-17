#ifndef BT_CONDITION_H
#define BT_CONDITION_H

#include "Leaf.h"
#include <functional>

namespace fluentBehaviorTree
{
	class Condition : public Leaf
	{
	private:
		Node* copy() override;
		// Function to check
		//bool(*mCondition)();
		std::function<bool()> mCondition;

	public:
		//Condition(std::string name, bool(*f)());
		Condition(std::string name, std::function<bool()> f);

	protected:
		EStatus tickNode() override;
	};
}

#endif