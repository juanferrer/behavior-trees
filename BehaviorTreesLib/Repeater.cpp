#include "Repeater.h"

namespace fluentBehaviorTree
{
	Repeater::Repeater(std::string name, int times = 0)
	{
		this->setName(name);
		mN = times;
	}

	// Repeat n times and return
	EStatus Repeater::tickNode()
	{
		if (mN == 0)
		{
			while (true)
			{
				this->setResult(mChild->tick());

				if (this->getResult() == EStatus::ERROR) return this->getResult();
			}
		}
		else
		{
			for (int i = 0; i < mN; ++i)
			{
				do
				{
					this->setResult(mChild->tick());
					if (this->getResult() == EStatus::ERROR) return this->getResult();
				} while (mChild->isOpen());
			}
		}
		this->setResult(EStatus::SUCCESS);
		return this->getResult();
	}
}