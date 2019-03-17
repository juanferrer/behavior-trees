#include "Action.h"

namespace fluentBehaviorTree
{
	Node * Action::copy()
	{
		Action* newNode = new Action(this->getName(), this->mAction);
		return newNode;
	}
	//Action::Action(std::string name, EStatus(*f)())
	Action::Action(std::string name, std::function<EStatus()> f)
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
