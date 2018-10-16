#include "Sequence.h"

namespace fluentBehaviorTree
{
	Node * Sequence::copy()
	{
		Sequence* newNode = new Sequence(this->getName());
		for (int i = 0, size = mChildren.size(); i < size; ++i)
		{
			newNode->addChild((mChildren[i]->copy()));
		}
		return newNode;
	}

	Sequence::Sequence(std::string name)
	{
		this->setName(name);
	}
	// Deallocate memory for children
	Sequence::~Sequence()
	{
		for each (auto n in mChildren)
		{
			if (n != nullptr)
			{
				delete n;
			}
		}
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