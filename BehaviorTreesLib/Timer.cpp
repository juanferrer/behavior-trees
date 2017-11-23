#include "Timer.h"
#include <thread>
#include <chrono>

namespace fluentBehaviorTree
{

	void Timer::onTimeout()
	{
		mInnerResult = mChild->tick();
		timerSet = false;
	}

	void Timer::setTimer()
	{
		std::this_thread::sleep_for(std::chrono::milliseconds(mMS));
		this->onTimeout();
	}

	Timer::Timer(std::string name, size_t ms)
	{
		this->setName(name);
		mMS = ms;
		mInnerResult = EStatus::RUNNING;
		timerSet = false;
	}

	// Reset its inner state
	void Timer::open()
	{
		Node::open();
		mInnerResult = EStatus::RUNNING;
		timerSet = false;
	}

	// Wait until ms is reached and run child. Return running during that time
	EStatus Timer::tickNode()
	{
		if (timerSet)
		{
			this->setResult(EStatus::RUNNING);
		}
		else if (mInnerResult != EStatus::RUNNING)
		{
			this->setResult(mInnerResult);
		}
		else
		{
			if (!timerSet)
			{
				std::thread timer(&Timer::setTimer, this);
				timerSet = true;
				timer.detach();
			}

			this->setResult(EStatus::RUNNING);
		}
		return this->getResult();
	}
}