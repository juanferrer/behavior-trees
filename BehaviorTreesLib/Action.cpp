#include "Action.h"

namespace fluentBehaviorTree
{
	Action::Action(std::string name, EStatus(*f)())
	{
		this->setName(name);
		mAction = f;
	}

	// Return result of action
	EStatus Action::tickNode()
	{
		try
		{
			this->setResult(mAction());
		}
		catch (std::exception& e)
		{
			this->setResult(EStatus::ERROR);
		}
		return this->getResult();
	}
}
