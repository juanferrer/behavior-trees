#ifndef BT_SUCCEEDER_H
#define BT_SUCCEEDER_H

#include "Decorator.h"

namespace fluentBehaviorTree
{
	class Succeeder : public Decorator
	{
	public:
		Succeeder(std::string name);
		~Succeeder() override;

	protected:
		EStatus tickNode() override;
	};
}

#endif