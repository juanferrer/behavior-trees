#include "Condition.h"
#include <string>

namespace fluentBehaviorTree
{
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