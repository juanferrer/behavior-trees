#ifndef BT_SELECTOR_H
#define BT_SELECTOR_H

#include "Composite.h"

namespace fluentBehaviorTree
{
	class Selector : public Composite
	{
	public:
		Selector(std::string name);

	protected:
		EStatus tickNode() override;
	};
}

#endif
