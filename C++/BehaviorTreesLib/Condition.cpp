#include "Condition.h"
#include <string>
#include <functional>

namespace fluentBehaviorTree
{
	Node * Condition::copy()
	{
		Condition* newNode = new Condition(this->getName(), this->mCondition);
		return newNode;
	}
	//Condition::Condition(std::string name, bool(*f)())
	Condition::Condition(std::string name, std::function<bool()> f)
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