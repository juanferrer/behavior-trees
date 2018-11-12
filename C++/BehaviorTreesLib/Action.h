#ifndef BT_ACTION_H
#define BT_ACTION_H

#include "Leaf.h"

namespace fluentBehaviorTree
{
	class Action : public Leaf
	{
	private:
		Node* copy() override;
		EStatus(*mAction)();

	public:
		Action(std::string name, EStatus(*f)());

	protected:
		EStatus tickNode() override;
	};
}

#endif