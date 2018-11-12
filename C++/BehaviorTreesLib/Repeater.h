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
		Node* copy() override;

		Repeater(std::string name, int times);
		~Repeater() override;

	protected:
		EStatus tickNode() override;
	};
}

#endif