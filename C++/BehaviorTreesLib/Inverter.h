#ifndef BT_INVERTER_H
#define BT_INVERTER_H

#include "Decorator.h"

namespace fluentBehaviorTree
{
	class Inverter : public Decorator
	{
	public:
		Node* copy() override;

		Inverter(std::string name);
		~Inverter() override;
	protected:
		EStatus tickNode() override;
	};
}

#endif