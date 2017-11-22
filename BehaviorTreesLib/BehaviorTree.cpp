#include "BehaviorTree.h"

namespace fluentBehaviorTree
{
	BehaviorTree::BehaviorTree(std::string name, Node & root)
	{
		this->setName(name);
		mRoot = &root;
	}
	EStatus BehaviorTree::tick()
	{
		return mRoot->tick();	
	}
}