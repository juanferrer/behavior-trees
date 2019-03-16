#include "Succeeder.h"

namespace fluentBehaviorTree
{
	Node * Succeeder::copy()
	{
		Succeeder* newNode = new Succeeder(this->getName());
		newNode->addChild(((this->mChild)->copy()));
		return newNode;
	}
	Succeeder::Succeeder(std::string name)
	{
		this->setName(name);
	}

	Succeeder::~Succeeder()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}

	// Regardless of result, return SUCCESS
	EStatus Succeeder::tickNode()
	{
		this->setResult(mChild->tick());

		if (this->getResult() == EStatus::FAILURE)
		{
			this->setResult(EStatus::SUCCESS);
		}
		return this->getResult();
	}
}