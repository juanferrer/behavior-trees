#include "Repeater.h"

namespace fluentBehaviorTree
{
	Node * Repeater::copy()
	{
		Repeater* newNode = new Repeater(this->getName(), this->mN);
		newNode->addChild(((this->mChild)->copy()));
		return newNode;
	}
	Repeater::Repeater(std::string name, int times = 0)
	{
		this->setName(name);
		mN = times;
	}

	Repeater::~Repeater()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}

	// Repeat n times and return
	EStatus Repeater::tickNode()
	{
		this->setResult(mChild->tick());
		// If something went wrong, crash here
		if (this->getResult() == EStatus::ERROR) return this->getResult();
		// If the child completed, count as an attempt
		if (this->getResult() != EStatus::RUNNING) ++mAttempts;

		if (mAttempts >= mN)
		{
			// This was last tick, return SUCCESS
			this->setResult(EStatus::SUCCESS);
		}
		else if (mAttempts < mN || mN == 0)
		{
			// Needs to keep going or it repeats forever, return RUNNING
			this->setResult(EStatus::RUNNING);
		}
		

		return this->getResult();
	}
}