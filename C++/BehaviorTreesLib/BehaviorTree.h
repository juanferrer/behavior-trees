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
		~BehaviorTree();

		std::string getName() { return mName; }
		Node* getRoot() { return mRoot; }
	
		EStatus tick();


	protected:
		void setName(std::string name) { mName = name; }
	};
}

#endif