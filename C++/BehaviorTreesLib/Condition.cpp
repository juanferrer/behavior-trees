#include "Condition.h"
#include <string>

namespace fluentBehaviorTree
{
	Node * Condition::copy()
	{
		Condition* newRoot = new Condition(this->getName(), this->mCondition);
		return newRoot;
	}
	Condition::Condition(std::string name, bool(*f)())
	{
		this->setName(name);
		mCondition = f;
	}

	// Return SUCCESS if the condition if met. Otherwise, FAILURE.
	EStatus Condition::tickNode()
	{
		try
		{
			this->setResult(mCondition() ? EStatus::SUCCESS : EStatus::FAILURE);
		}
		catch (const std::exception& e)
		{
			this->setResult(EStatus::ERROR);
		}
		return this->getResult();
	}
}