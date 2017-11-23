#ifndef BT_REPEAT_UNTIL_FAIL_H
#define BT_REPEAT_UNTIL_FAIL_H

#include "Decorator.h"

namespace fluentBehaviorTree
{
	class RepeatUntilFail : public Decorator
	{
	public:
		RepeatUntilFail(std::string name);
		~RepeatUntilFail() override;

	protected:
		EStatus tickNode() override;
	};
}

#endif