#include "Succeeder.h"

namespace fluentBehaviorTree
{
	Succeeder::Succeeder(std::string name)
	{
		this->setName(name);
	}

	Succeeder::~Succeeder()
	{
		if (mChild != nullptr)
		{
			delete mChild;
		}
	}

	// Regardless of result, return SUCCESS
	EStatus Succeeder::tickNode()
	{
		this->setResult(mChild->tick());

		if (this->getResult() != EStatus::ERROR)
		{
			this->setResult(EStatus::SUCCESS);
		}
		return this->getResult();
	}
}