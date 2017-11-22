#ifndef BT_INVERTER_H
#define BT_INVERTER_H

#include "Decorator.h"

namespace fluentBehaviorTree
{
	class Inverter : public Decorator
	{
	public:
		Inverter(std::string name);
	protected:
		EStatus tickNode() override;
	};
}

#endif