#include "Inverter.h"

namespace fluentBehaviorTree
{
	Inverter::Inverter(std::string name)
	{
		this->setName(name);
	}

	Inverter::~Inverter()
	{
		if (mChild != nullptr)
		{
			delete mChild;
			mChild = nullptr;
		}
	}

	// Works as NOT logic operator
	EStatus Inverter::tickNode()
	{
		this->setResult(mChild->tick());

		if (this->getResult() == EStatus::SUCCESS) this->setResult(EStatus::FAILURE);
		else if (this->getResult() == EStatus::FAILURE) this->setResult(EStatus::SUCCESS);

		return this->getResult();
	}
}