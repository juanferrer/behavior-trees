#ifndef BT_TIMER_H
#define BT_TIMER_H

#include "Decorator.h"

namespace fluentBehaviorTree
{
	class Timer : public Decorator
	{
	private:
		int mTimeElapsed;
		int mMS;
		EStatus mInnerResult;
		void setTimer();
		void onTimeout();
		bool timerSet;

	public:
		Timer(std::string name, size_t ms);

	protected:
		void open() override;
		EStatus tickNode() override;
	};

}

#endif