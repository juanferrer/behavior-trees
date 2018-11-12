#include "BehaviorTree.h"
#include "Root.h"

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

	BehaviorTree::~BehaviorTree()
	{
		if (mRoot != nullptr)
		{
			delete mRoot;
			mRoot = nullptr;
		}
	}

	// Tick tree to navigate and get result
	EStatus BehaviorTree::tick()
	{
		return mRoot->tick();	
	}
}