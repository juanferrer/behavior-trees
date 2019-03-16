using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBehaviorTree;

namespace BTTests
{
    [TestClass]
    public class APITests
    {
        // Leaf nodes
        [TestMethod]
        public void ActionTest()
        {
            Assert.AreEqual(Status.SUCCESS, Action(() => { return Status.SUCCESS; }));
            Assert.AreEqual(Status.FAILURE, Action(() => { return Status.FAILURE; }));
            Assert.AreEqual(Status.RUNNING, Action(() => { return Status.RUNNING; }));
            Assert.AreEqual(Status.ERROR, Action(() => { return Status.ERROR; }));
        }

        [TestMethod]
        public void ConditionTest()
        {
            Assert.AreEqual(Status.SUCCESS, Condition(() => { return true; }));
            Assert.AreEqual(Status.FAILURE, Condition(() => { return false; }));
        }

        [TestMethod]
        public void SubtreeTest()
        {
            Assert.AreEqual(Status.SUCCESS, Subtree(() => { return Status.SUCCESS; }));
            Assert.AreEqual(Status.FAILURE, Subtree(() => { return Status.FAILURE; }));
            Assert.AreEqual(Status.RUNNING, Subtree(() => { return Status.RUNNING; }));
            Assert.AreEqual(Status.ERROR, Subtree(() => { return Status.ERROR; }));
        }

        // Composites
        [TestMethod]
        public void SequenceTest()
        {
            // Only succeeds when all are true
            Assert.AreEqual(Status.SUCCESS, Sequence(true, true, true));
            Assert.AreEqual(Status.FAILURE, Sequence(true, true, false));
            Assert.AreEqual(Status.FAILURE, Sequence(true, false, false));
            Assert.AreEqual(Status.FAILURE, Sequence(false, false, false));
            Assert.AreEqual(Status.FAILURE, Sequence(false, true, true));
            Assert.AreEqual(Status.FAILURE, Sequence(false, true, false));
            Assert.AreEqual(Status.FAILURE, Sequence(true, false, true));
            Assert.AreEqual(Status.FAILURE, Sequence(false, false, true));
        }

        [TestMethod]
        public void SelectorTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Status.SUCCESS, Selector(true, true, true));
            Assert.AreEqual(Status.SUCCESS, Selector(true, true, false));
            Assert.AreEqual(Status.SUCCESS, Selector(true, false, false));
            Assert.AreEqual(Status.FAILURE, Selector(false, false, false));
            Assert.AreEqual(Status.SUCCESS, Selector(false, true, true));
            Assert.AreEqual(Status.SUCCESS, Selector(false, true, false));
            Assert.AreEqual(Status.SUCCESS, Selector(true, false, true));
            Assert.AreEqual(Status.SUCCESS, Selector(false, false, true));
        }

        // Decorators
        [TestMethod]
        public void NegatorTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Status.FAILURE, Negator(() => { return Status.SUCCESS; }));
            Assert.AreEqual(Status.SUCCESS, Negator(() => { return Status.FAILURE; }));
            Assert.AreEqual(Status.RUNNING, Negator(() => { return Status.RUNNING; }));
            Assert.AreEqual(Status.ERROR, Negator(() => { return Status.ERROR; }));
        }

        [TestMethod]
        public void RepeaterTest()
        {
            Assert.AreEqual(Status.SUCCESS, Repeater(5, 5));
            Assert.AreEqual(Status.SUCCESS, Repeater(2, 5));
            Assert.AreEqual(Status.SUCCESS, Repeater(0, 5));
            Assert.AreEqual(Status.RUNNING, Repeater(5, 1));
            Assert.AreEqual(Status.ERROR, Repeater(0, 0));
        }

        [TestMethod]
        public void RepeatUntilFailTest()
        {
            Assert.AreEqual(Status.SUCCESS, RepeatUntilFail(5, 5));
            Assert.AreEqual(Status.SUCCESS, RepeatUntilFail(2, 5));
            Assert.AreEqual(Status.SUCCESS, RepeatUntilFail(0, 5));
            Assert.AreEqual(Status.RUNNING, RepeatUntilFail(5, 1));
            Assert.AreEqual(Status.ERROR, RepeatUntilFail(0, 0));
        }

        [TestMethod]
        public void SucceederTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Status.SUCCESS, Succeeder(() => { return Status.SUCCESS; }));
            Assert.AreEqual(Status.SUCCESS, Succeeder(() => { return Status.FAILURE; }));
            Assert.AreEqual(Status.RUNNING, Succeeder(() => { return Status.RUNNING; }));
            Assert.AreEqual(Status.ERROR, Succeeder(() => { return Status.ERROR; }));
        }

        [TestMethod]
        public void TimerTest()
        {
            Assert.AreEqual(Status.SUCCESS, Timer(100, 100));
            Assert.AreEqual(Status.RUNNING, Timer(300, 100));
            Assert.AreEqual(Status.SUCCESS, Timer(100, 300));
        }

        Status Action(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("ActionTest")
                .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Condition(Func<bool> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("ConditionTest")
                .If("Condition", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Subtree(Func<Status> function)
        {
            BehaviorTree subtree = new BehaviorTreeBuilder("Subtree")
                .Do("Action", function)
                .End();

            BehaviorTree bt = new BehaviorTreeBuilder("SubtreeTest")
                .Do("Action", subtree)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Sequence(bool b1, bool b2, bool b3)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("SequenceTest")
                .Sequence("Sequence")
                    .If("Condition", () => { return b1; })
                    .If("Condition", () => { return b2; })
                    .If("Condition", () => { return b3; })
                    .End()
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Selector(bool n1, bool n2, bool n3)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("SelectorTest")
                .Selector("Selector")
                    .If("Condition", () => { return n1; })
                    .If("Condition", () => { return n2; })
                    .If("Condition", () => { return n3; })
                    .End()
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Negator(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("NegatorTest")
                .Not("Negator")
                    .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Repeater(int timesUntilSuccess, int timesToTick)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("RepeaterTest")
                .Repeat("Repeater", timesUntilSuccess)
                    .Do("Action", () =>{ return Status.SUCCESS; })
                .End();
            Status result = Status.ERROR;
            for (int i = 0; i < timesToTick; ++i)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status RepeatUntilFail(int timesUntilFail, int timesToTick)
        {
            int attempts = 0;
            BehaviorTree bt = new BehaviorTreeBuilder("RepeatUntilFailTest")
                .RepeatUntilFail("RepeatUntilFail")
                    .Do("Action", () =>
                    {
                        if (attempts < timesUntilFail - 1)
                        {
                            attempts++;
                            return Status.SUCCESS;
                        }
                        else
                        {
                            return Status.FAILURE;
                        }
                    })
                .End();
            Status result = Status.ERROR;
            for (int i = 0; i < timesToTick; ++i)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status Succeeder(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("SucceederTest")
                .Ignore("Succeeder")
                    .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Timer(ulong timeUntilSuccess, int waitTime)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("TimerTest")
                .Wait("Timer", timeUntilSuccess)
                    .Do("Action", () => { return Status.SUCCESS; })
                .End();
            Status result = bt.Tick();
            // Sleep for 50 ms longer, just to make sure the task finishes
            System.Threading.Thread.Sleep(waitTime + 50);
            result = bt.Tick();

            return result;
        }
    }

    [TestClass]
    public class BTMLTests
    {
        [TestMethod]
        public void Success()
        {
            return;
        }
    }
}
