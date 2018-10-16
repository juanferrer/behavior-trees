#include "Root.h"

namespace fluentBehaviorTree
{
	void Root::addChild(Node* n)
	{
		mChild = n;
	}
	Root::Root()
	{
		this->setName("Root");
	}

	Root::~Root()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}

	// Ticks only child and returns
	EStatus Root::tickNode()
	{
		this->setResult(mChild->tick());

		return this->getResult();
	}

	// Perform deep copy of tree and return a pointer to the new tree
	Node * Root::copy()
	{
		Root* newNode = new Root();
		newNode->addChild(((this->mChild)->copy()));
		return newNode;
	}
}