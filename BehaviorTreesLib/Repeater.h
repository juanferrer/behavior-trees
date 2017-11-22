#ifndef BT_REPEATER_H
#define BT_REPEATER_H

#include "Decorator.h"

namespace fluentBehaviorTree
{
	class Repeater : public Decorator
	{
	private:
		int mN;

	public:
		Repeater(std::string name, int times);

	protected:
		EStatus tickNode() override;
	};
}

#endif