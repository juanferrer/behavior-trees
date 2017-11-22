#include "Selector.h"

namespace fluentBehaviorTree
{
	Selector::Selector(std::string name)
	{
		setName(name);
	}

	// Propagate tick to children.Return FAILURE if no child succeeds
	EStatus Selector::tickNode()
	{
		if (this->getResult() == EStatus::RUNNING)
		{
			for each (auto n in mChildren)
			{
				if (!n->isOpen() && n->getResult() == EStatus::SUCCESS)
				{
					this->setResult(EStatus::SUCCESS);
					return this->getResult();
				}
				else if (n->getResult() == EStatus::RUNNING)
				{
					this->setResult(n->tick());
					if (this->getResult() != EStatus::FAILURE) return this->getResult();
				}
			}
			this->setResult(EStatus::FAILURE);
		}
		return this->getResult();
	}
}