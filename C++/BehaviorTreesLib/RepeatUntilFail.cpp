#include "RepeatUntilFail.h"

namespace fluentBehaviorTree
{
	Node * RepeatUntilFail::copy()
	{
		RepeatUntilFail* newNode = new RepeatUntilFail(this->getName());
		newNode->addChild(((this->mChild)->copy()));
		return newNode;
	}
	RepeatUntilFail::RepeatUntilFail(std::string name)
	{
		this->setName(name);
	}

	RepeatUntilFail::~RepeatUntilFail()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}

	// Repeat until FAILURE
	EStatus RepeatUntilFail::tickNode()
	{
		do
		{
			this->setResult(mChild->tick());
			if (this->getResult() == EStatus::ERROR) return this->getResult();
		} while (this->getResult() != EStatus::FAILURE);

		this->setResult(EStatus::SUCCESS);
		return this->getResult();
	}
}