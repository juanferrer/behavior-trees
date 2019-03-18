#include "stdafx.h"
#include "CppUnitTest.h"
#include "../BehaviorTreesLib/BehaviorTree.h"
#include "../BehaviorTreesLib/BehaviorTreeBuilder.h"
#include <thread>
#include <chrono>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace fluentBehaviorTree;

namespace Microsoft
{
	namespace VisualStudio
	{
		namespace CppUnitTestFramework
		{
			template<> static std::wstring ToString<EStatus>(const EStatus& t)
			{
				switch (t)
				{
					case EStatus::SUCCESS:
						return L"EStatus::SUCCESS";
						break;
					case EStatus::FAILURE:
						return L"EStatus::FAILURE";
						break;
					case EStatus::RUNNING:
						return L"EStatus::RUNNING";
						break;
					case EStatus::ERROR:
						return L"EStatus::ERROR";
						break;
					default:
						return L"EStatus";
						break;
				}
			}
		}
	}
}

namespace BTTestsCPP
{
	TEST_CLASS(APITests)
	{
	public:

		TEST_METHOD(ActionTest)
		{
			Assert::AreEqual(EStatus::SUCCESS, Action([]() { return EStatus::SUCCESS; }));
			Assert::AreEqual(EStatus::FAILURE, Action([]() { return EStatus::FAILURE; }));
			Assert::AreEqual(EStatus::RUNNING, Action([]() { return EStatus::RUNNING; }));
			Assert::AreEqual(EStatus::ERROR,   Action([]() { return EStatus::ERROR; }));

		}

		TEST_METHOD(ConditionTest)
		{
			Assert::AreEqual(EStatus::SUCCESS, Condition([]() { return true; }));
			Assert::AreEqual(EStatus::FAILURE, Condition([]() { return false; }));
		}

		TEST_METHOD(SubtreeTest)
		{
			Assert::AreEqual(EStatus::SUCCESS, Subtree([]() { return EStatus::SUCCESS; }));
			Assert::AreEqual(EStatus::FAILURE, Subtree([]() { return EStatus::FAILURE; }));
			Assert::AreEqual(EStatus::RUNNING, Subtree([]() { return EStatus::RUNNING; }));
			Assert::AreEqual(EStatus::ERROR,   Subtree([]() { return EStatus::ERROR; }));
		}

		// Composites
		TEST_METHOD(SequenceTest)
		{
			// Only succeeds when all are true
			Assert::AreEqual(EStatus::SUCCESS, Sequence(true, true, true));
			Assert::AreEqual(EStatus::FAILURE, Sequence(true, true, false));
			Assert::AreEqual(EStatus::FAILURE, Sequence(true, false, false));
			Assert::AreEqual(EStatus::FAILURE, Sequence(false, false, false));
			Assert::AreEqual(EStatus::FAILURE, Sequence(false, true, true));
			Assert::AreEqual(EStatus::FAILURE, Sequence(false, true, false));
			Assert::AreEqual(EStatus::FAILURE, Sequence(true, false, true));
			Assert::AreEqual(EStatus::FAILURE, Sequence(false, false, true));
		}

		TEST_METHOD(SelectorTest)
		{
			// Only fails when all are false
			Assert::AreEqual(EStatus::SUCCESS, Selector(true, true, true));
			Assert::AreEqual(EStatus::SUCCESS, Selector(true, true, false));
			Assert::AreEqual(EStatus::SUCCESS, Selector(true, false, false));
			Assert::AreEqual(EStatus::FAILURE, Selector(false, false, false));
			Assert::AreEqual(EStatus::SUCCESS, Selector(false, true, true));
			Assert::AreEqual(EStatus::SUCCESS, Selector(false, true, false));
			Assert::AreEqual(EStatus::SUCCESS, Selector(true, false, true));
			Assert::AreEqual(EStatus::SUCCESS, Selector(false, false, true));
		}

		// Decorators
		TEST_METHOD(NegatorTest)
		{
			// Only fails when all are false
			Assert::AreEqual(EStatus::FAILURE, Negator([]() { return EStatus::SUCCESS; }));
			Assert::AreEqual(EStatus::SUCCESS, Negator([]() { return EStatus::FAILURE; }));
			Assert::AreEqual(EStatus::RUNNING, Negator([]() { return EStatus::RUNNING; }));
			Assert::AreEqual(EStatus::ERROR,   Negator([]() { return EStatus::ERROR; }));
		}

		TEST_METHOD(RepeaterTest)
		{
			Assert::AreEqual(EStatus::SUCCESS, Repeater(5, 5));
			Assert::AreEqual(EStatus::SUCCESS, Repeater(2, 5));
			Assert::AreEqual(EStatus::SUCCESS, Repeater(0, 5));
			Assert::AreEqual(EStatus::RUNNING, Repeater(5, 1));
			Assert::AreEqual(EStatus::ERROR,   Repeater(0, 0));
		}

		TEST_METHOD(RepeatUntilFailTest)
		{
			Assert::AreEqual(EStatus::SUCCESS, RepeatUntilFail(5, 5));
			Assert::AreEqual(EStatus::SUCCESS, RepeatUntilFail(2, 5));
			Assert::AreEqual(EStatus::SUCCESS, RepeatUntilFail(0, 5));
			Assert::AreEqual(EStatus::RUNNING, RepeatUntilFail(5, 1));
			Assert::AreEqual(EStatus::ERROR,   RepeatUntilFail(0, 0));
		}

		TEST_METHOD(SucceederTest)
		{
			// Only fails when all are false
			Assert::AreEqual(EStatus::SUCCESS, Succeeder([]() { return EStatus::SUCCESS; }));
			Assert::AreEqual(EStatus::SUCCESS, Succeeder([]() { return EStatus::FAILURE; }));
			Assert::AreEqual(EStatus::RUNNING, Succeeder([]() { return EStatus::RUNNING; }));
			Assert::AreEqual(EStatus::ERROR,   Succeeder([]() { return EStatus::ERROR; }));
		}

		TEST_METHOD(TimerTest)
		{
			Assert::AreEqual(EStatus::SUCCESS, Timer(100, 100));
			Assert::AreEqual(EStatus::RUNNING, Timer(300, 100));
			Assert::AreEqual(EStatus::SUCCESS, Timer(100, 300));
		}

	private:
		EStatus Action(EStatus(*function)())
		{
			BehaviorTree bt = BehaviorTreeBuilder("ActionTest")
				.Do("Action", function)
				.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Condition(bool (*function)())
		{
			BehaviorTree bt = BehaviorTreeBuilder("ConditionTest")
				.If("Condition", function)
				.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Subtree(EStatus (*function)())
		{
			BehaviorTree subtree = BehaviorTreeBuilder("Subtree")
				.Do("Action", function)
				.End();

			BehaviorTree bt = BehaviorTreeBuilder("SubtreeTest")
				.Do("Action", subtree.getRoot())
				.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Sequence(bool b1, bool b2, bool b3)
		{

			BehaviorTree bt = BehaviorTreeBuilder("SequenceTest")
				.Sequence("Sequence")
					.If("Condition", [b1]() { return b1; })
					.If("Condition", [b2]() { return b2; })
					.If("Condition", [b3]() { return b3; })
					.End()
				.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Selector(bool b1, bool b2, bool b3)
		{

			BehaviorTree bt = BehaviorTreeBuilder("SelectorTest")
				.Selector("Selector")
					.If("Condition", [b1]() { return b1; })
					.If("Condition", [b2]() { return b2; })
					.If("Condition", [b3]() { return b3; })
					.End()
				.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Negator(EStatus(*function)())
		{
			BehaviorTree bt = BehaviorTreeBuilder("NegatorTest")
				.Not("Negator")
					.Do("Action", function)
					.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Repeater(int timesUntilSuccess, int timesTotick)
		{
			BehaviorTree bt = BehaviorTreeBuilder("RepeaterTest")
				.Repeat("Repeater", timesUntilSuccess)
					.Do("Action", []() { return EStatus::SUCCESS; })
					.End();
			EStatus result = EStatus::ERROR;
			for (int i = 0; i < timesTotick; ++i)
			{
				result = bt.tick();
			}

			return result;
		}

		EStatus RepeatUntilFail(int timesUntilFail, int timesTotick)
		{
			int attempts = 0;
			std::function<EStatus()> function = [&attempts, timesUntilFail]()
			{
				if (attempts < timesUntilFail - 1)
				{
					attempts++;
					return EStatus::SUCCESS;
				}
				else
				{
					return EStatus::FAILURE;
				}
			};

			BehaviorTree bt = BehaviorTreeBuilder("RepeatUntilFailTest")
				.RepeatUntilFail("RepeatUntilFail")
					.Do("Action", function)
					.End();
			EStatus result = EStatus::ERROR;
			for (int i = 0; i < timesTotick; ++i)
			{
				result = bt.tick();
			}

			return result;
		}

		EStatus Succeeder(EStatus (*function)())
		{
			BehaviorTree bt = BehaviorTreeBuilder("SucceederTest")
				.Ignore("Succeeder")
					.Do("Action", function)
				.End();

			EStatus result = bt.tick();

			return result;
		}

		EStatus Timer(size_t timeUntilSuccess, int waitTime)
		{
			BehaviorTree bt = BehaviorTreeBuilder("TimerTest")
				.Wait("Timer", timeUntilSuccess)
					.Do("Action", []() { return EStatus::SUCCESS; })
				.End();
			EStatus result = bt.tick();
			// Sleep for 50 ms longer, just to make sure the task finishes
			std::this_thread::sleep_for(std::chrono::milliseconds(waitTime + 50));
			result = bt.tick();

			return result;
		}
	};
}