#ifndef BT_BEHAVIOR_TREE_H
#define BT_BEHAVIOR_TREE_H

#include <string>

#include "Node.h"
#include "General.h"
#include "BehaviorTreeBuilder.h"

namespace fluentBehaviorTree
{
	class BehaviorTree
	{
	private:
		std::string mName;
		Node* mRoot;

	public:

		BehaviorTree(std::string name, Node* root);
		BehaviorTree(BehaviorTreeBuilder btb);

		std::string getName() { return mName; }

		// Tick tree to navigate and get result
		EStatus tick();


	protected:
		void setName(std::string name) { mName = name; }
	};
}

#endif