#include "Sequence.h"

namespace fluentBehaviorTree
{
	Sequence::Sequence(std::string name)
	{
		this->setName(name);
	}

	// Propagate tick to children. Return SUCCESS if no child fails
	EStatus Sequence::tickNode()
	{
		if (this->getResult() == EStatus::RUNNING)
		{
			for each(auto n in mChildren)
			{
				if (!n->isOpen() && n->getResult() == EStatus::FAILURE)
				{
					this->setResult(EStatus::FAILURE);
					return this->getResult();
				}
				else if (n->getResult() == EStatus::RUNNING)
				{
					this->setResult(n->tick());
					if (this->getResult() != EStatus::SUCCESS) return this->getResult();
				}
			}
			this->setResult(EStatus::SUCCESS);
		}
		return this->getResult();
	}
}