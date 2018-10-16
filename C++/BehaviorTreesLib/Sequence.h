#ifndef BT_SEQUENCE_H
#define BT_SEQUENCE_H

#include "Composite.h"

namespace fluentBehaviorTree
{
	class Sequence : public Composite
	{
	public:
		Node* copy() override;
		Sequence(std::string name);

		~Sequence() override;

	protected:
		EStatus tickNode() override;
	};
}

#endif
