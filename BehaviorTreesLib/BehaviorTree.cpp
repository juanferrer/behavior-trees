#include "BehaviorTree.h"

namespace fluentBehaviorTree
{
	BehaviorTree::BehaviorTree(std::string name, Node* root)
	{
		this->setName(name);
		mRoot = root;
	}

	BehaviorTree::BehaviorTree(BehaviorTreeBuilder btb)
	{
		this->setName(btb.getName());
		mRoot = btb.getRoot();
	}

	EStatus BehaviorTree::tick()
	{
		return mRoot->tick();	
	}
}