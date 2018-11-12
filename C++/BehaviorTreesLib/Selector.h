#ifndef BT_SELECTOR_H
#define BT_SELECTOR_H

#include "Composite.h"

namespace fluentBehaviorTree
{
	class Selector : public Composite
	{
	public:
		Node* copy() override;
		Selector(std::string name);

		~Selector() override;

	protected:
		EStatus tickNode() override;
	};
}

#endif
