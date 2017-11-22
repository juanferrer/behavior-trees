#ifndef BT_SEQUENCE_H
#define BT_SEQUENCE_H

#include "Composite.h"

namespace fluentBehaviorTree
{
	class Sequence : public Composite
	{
	public:
		Sequence(std::string name);

	protected:
		EStatus tickNode() override;
	};
}

#endif
