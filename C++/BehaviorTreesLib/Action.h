#ifndef BT_ACTION_H
#define BT_ACTION_H

#include "Leaf.h"
#include <functional>

namespace fluentBehaviorTree
{
	class Action : public Leaf
	{
	private:
		Node* copy() override;
		//EStatus(*mAction)();
		std::function<EStatus()> mAction;

	public:
		//Action(std::string name, EStatus(*f)());
		Action(std::string name, std::function<EStatus()> f);

	protected:
		EStatus tickNode() override;
	};
}

#endif