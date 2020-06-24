using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBehaviorTree;

namespace BTTests
{
    [TestClass]
    public class FluentAPITests
    {
        // Leaf nodes
        [TestMethod]
        public void ActionTest()
        {
            Assert.AreEqual(Action(ReturnSUCCESS), ActionAPI(ReturnSUCCESS));
            Assert.AreEqual(Action(ReturnFAILURE), ActionAPI(ReturnFAILURE));
            Assert.AreEqual(Action(ReturnRUNNING), ActionAPI(ReturnRUNNING));
            Assert.AreEqual(Action(ReturnERROR),   ActionAPI(ReturnERROR));
        }

        [TestMethod]
        public void ConditionTest()
        {
            Assert.AreEqual(Condition(ReturnTrue),  ConditionAPI(ReturnTrue));
            Assert.AreEqual(Condition(ReturnFalse), ConditionAPI(ReturnFalse));
        }

        [TestMethod]
        public void SubtreeTest()
        {
            Assert.AreEqual(Subtree(ReturnSUCCESS), SubtreeAPI(ReturnSUCCESS));
            Assert.AreEqual(Subtree(ReturnFAILURE), SubtreeAPI(ReturnFAILURE));
            Assert.AreEqual(Subtree(ReturnRUNNING), SubtreeAPI(ReturnRUNNING));
            Assert.AreEqual(Subtree(ReturnERROR),   SubtreeAPI(ReturnERROR));
        }

        // Composites
        [TestMethod]
        public void SequenceTest()
        {
            // Only succeeds when all are true
            Assert.AreEqual(Sequence(true,  true,  true),  SequenceAPI(true,  true,  true));
            Assert.AreEqual(Sequence(true,  true,  false), SequenceAPI(true,  true,  false));
            Assert.AreEqual(Sequence(true,  false, false), SequenceAPI(true,  false, false));
            Assert.AreEqual(Sequence(false, false, false), SequenceAPI(false, false, false));
            Assert.AreEqual(Sequence(false, true,  true),  SequenceAPI(false, true,  true));
            Assert.AreEqual(Sequence(false, true,  false), SequenceAPI(false, true,  false));
            Assert.AreEqual(Sequence(true,  false, true),  SequenceAPI(true,  false, true));
            Assert.AreEqual(Sequence(false, false, true),  SequenceAPI(false, false, true));
        }

        [TestMethod]
        public void SelectorTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Selector(true,  true,  true),  SelectorAPI(true,  true,  true));
            Assert.AreEqual(Selector(true,  true,  false), SelectorAPI(true,  true,  false));
            Assert.AreEqual(Selector(true,  false, false), SelectorAPI(true,  false, false));
            Assert.AreEqual(Selector(false, false, false), SelectorAPI(false, false, false));
            Assert.AreEqual(Selector(false, true,  true),  SelectorAPI(false, true,  true));
            Assert.AreEqual(Selector(false, true,  false), SelectorAPI(false, true,  false));
            Assert.AreEqual(Selector(true,  false, true),  SelectorAPI(true,  false, true));
            Assert.AreEqual(Selector(false, false, true),  SelectorAPI(false, false, true));
        }

        [TestMethod]
        public void RandomSequenceTest()
        {
            // Only fails when all are false
            Assert.AreEqual(RandomSequence(true, true, true),    RandomSequenceAPI(true, true, true));
            Assert.AreEqual(RandomSequence(true, true, false),   RandomSequenceAPI(true, true, false));
            Assert.AreEqual(RandomSequence(true, false, false),  RandomSequenceAPI(true, false, false));
            Assert.AreEqual(RandomSequence(false, false, false), RandomSequenceAPI(false, false, false));
            Assert.AreEqual(RandomSequence(false, true, true),   RandomSequenceAPI(false, true, true));
            Assert.AreEqual(RandomSequence(false, true, false),  RandomSequenceAPI(false, true, false));
            Assert.AreEqual(RandomSequence(true, false, true),   RandomSequenceAPI(true, false, true));
            Assert.AreEqual(RandomSequence(false, false, true),  RandomSequenceAPI(false, false, true));
        }

        [TestMethod]
        public void RandomSelectorTest()
        {
            // Only fails when all are false
            Assert.AreEqual(RandomSelector(true, true, true),    RandomSelectorAPI(true, true, true));
            Assert.AreEqual(RandomSelector(true, true, false),   RandomSelectorAPI(true, true, false));
            Assert.AreEqual(RandomSelector(true, false, false),  RandomSelectorAPI(true, false, false));
            Assert.AreEqual(RandomSelector(false, false, false), RandomSelectorAPI(false, false, false));
            Assert.AreEqual(RandomSelector(false, true, true),   RandomSelectorAPI(false, true, true));
            Assert.AreEqual(RandomSelector(false, true, false),  RandomSelectorAPI(false, true, false));
            Assert.AreEqual(RandomSelector(true, false, true),   RandomSelectorAPI(true, false, true));
            Assert.AreEqual(RandomSelector(false, false, true),  RandomSelectorAPI(false, false, true));
        }

        // Decorators
        [TestMethod]
        public void NegatorTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Negator(ReturnSUCCESS), NegatorAPI(ReturnSUCCESS));
            Assert.AreEqual(Negator(ReturnFAILURE), NegatorAPI(ReturnFAILURE));
            Assert.AreEqual(Negator(ReturnRUNNING), NegatorAPI(ReturnRUNNING));
            Assert.AreEqual(Negator(ReturnERROR),   NegatorAPI(ReturnERROR));
        }

        [TestMethod]
        public void RepeaterTest()
        {
            Assert.AreEqual(Repeater(5, 5), RepeaterAPI(5, 5));
            Assert.AreEqual(Repeater(2, 5), RepeaterAPI(2, 5));
            Assert.AreEqual(Repeater(0, 5), RepeaterAPI(0, 5));
            Assert.AreEqual(Repeater(5, 1), RepeaterAPI(5, 1));
            Assert.AreEqual(Repeater(0, 0), RepeaterAPI(0, 0));
        }

        [TestMethod]
        public void RepeatUntilFailTest()
        {
            Assert.AreEqual(RepeatUntilFail(5, 5), RepeatUntilFailAPI(5, 5));
            Assert.AreEqual(RepeatUntilFail(2, 5), RepeatUntilFailAPI(2, 5));
            Assert.AreEqual(RepeatUntilFail(0, 5), RepeatUntilFailAPI(0, 5));
            Assert.AreEqual(RepeatUntilFail(5, 1), RepeatUntilFailAPI(5, 1));
            Assert.AreEqual(RepeatUntilFail(0, 0), RepeatUntilFailAPI(0, 0));
        }

        [TestMethod]
        public void SucceederTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Succeeder(ReturnSUCCESS), SucceederAPI(ReturnSUCCESS));
            Assert.AreEqual(Succeeder(ReturnFAILURE), SucceederAPI(ReturnFAILURE));
            Assert.AreEqual(Succeeder(ReturnRUNNING), SucceederAPI(ReturnRUNNING));
            Assert.AreEqual(Succeeder(ReturnERROR),   SucceederAPI(ReturnERROR));
        }

        [TestMethod]
        public void TimerTest()
        {
            Assert.AreEqual(Timer(100, 100), TimerAPI(100, 100));
            Assert.AreEqual(Timer(300, 100), TimerAPI(300, 100));
            Assert.AreEqual(Timer(100, 300), TimerAPI(100, 300));
        }

        Status ReturnSUCCESS()
        {
            return Status.SUCCESS;
        }

        Status ReturnFAILURE()
        {
            return Status.FAILURE;
        }

        Status ReturnRUNNING()
        {
            return Status.RUNNING;
        }

        Status ReturnERROR()
        {
            return Status.ERROR;
        }

        bool ReturnTrue()
        {
            return true;
        }

        bool ReturnFalse()
        {
            return false;
        }

        Status ActionAPI(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("ActionTest")
                .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Action(Func<Status> function)
        {
            var root = new Root();
            root.AddChild(new FluentBehaviorTree.Action("Action", function));
            BehaviorTree bt = new BehaviorTree("ActionTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status ConditionAPI(Func<bool> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("ConditionTest")
                .If("Condition", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Condition(Func<bool> function)
        {
            var root = new Root();
            root.AddChild(new Condition("Condition", function));
            BehaviorTree bt = new BehaviorTree("ConditionTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status SubtreeAPI(Func<Status> function)
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

        Status Subtree(Func<Status> function)
        {
            var subtreeRoot = new Root();
            subtreeRoot.AddChild(new FluentBehaviorTree.Action("Action", function));
            BehaviorTree subtree = new BehaviorTree("SubtreeTest", subtreeRoot);

            var root = new Root();
            root.AddChild((subtree.Clone() as BehaviorTree).GetRoot());
            BehaviorTree bt = new BehaviorTree("SubtreeTest", root);


            Status result = bt.Tick();

            return result;
        }

        Status SequenceAPI(bool b1, bool b2, bool b3)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("SequenceTest")
                .Sequence("Sequence")
                    .If("Condition 1", () => { return b1; })
                    .If("Condition 2", () => { return b2; })
                    .If("Condition 3", () => { return b3; })
                    .End()
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Sequence(bool b1, bool b2, bool b3)
        {
            var root = new Root();
            var seq = new Sequence("Sequence");
            seq.AddChild(new Condition("Condition 1", () => { return b1; }));
            seq.AddChild(new Condition("Condition 2", () => { return b2; }));
            seq.AddChild(new Condition("Condition 3", () => { return b3; }));
            root.AddChild(seq);
            BehaviorTree bt = new BehaviorTree("SequenceTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status SelectorAPI(bool b1, bool b2, bool b3)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("SelectorTest")
                .Selector("Selector")
                    .If("Condition 1", () => { return b1; })
                    .If("Condition 2", () => { return b2; })
                    .If("Condition 3", () => { return b3; })
                    .End()
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Selector(bool b1, bool b2, bool b3)
        {
            var root = new Root();
            var seq = new Selector("Selector");
            seq.AddChild(new Condition("Condition 1", () => { return b1; }));
            seq.AddChild(new Condition("Condition 2", () => { return b2; }));
            seq.AddChild(new Condition("Condition 3", () => { return b3; }));
            root.AddChild(seq);
            BehaviorTree bt = new BehaviorTree("SelectorTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status RandomSequenceAPI(bool b1, bool b2, bool b3)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("RandomSequenceTest")
                .Sequence("Random sequence")
                    .If("Condition 1", () => { return b1; })
                    .If("Condition 2", () => { return b2; })
                    .If("Condition 3", () => { return b3; })
                    .End()
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status RandomSequence(bool b1, bool b2, bool b3)
        {
            var root = new Root();
            var seq = new RandomSequence("Random sequence");
            seq.AddChild(new Condition("Condition 1", () => { return b1; }));
            seq.AddChild(new Condition("Condition 2", () => { return b2; }));
            seq.AddChild(new Condition("Condition 3", () => { return b3; }));
            root.AddChild(seq);
            BehaviorTree bt = new BehaviorTree("RandomSequenceTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status RandomSelectorAPI(bool b1, bool b2, bool b3)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("RandomSelectorTest")
                .Selector("Random selector")
                    .If("Condition 1", () => { return b1; })
                    .If("Condition 2", () => { return b2; })
                    .If("Condition 3", () => { return b3; })
                    .End()
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status RandomSelector(bool b1, bool b2, bool b3)
        {
            var root = new Root();
            var seq = new RandomSelector("Random selector");
            seq.AddChild(new Condition("Condition 1", () => { return b1; }));
            seq.AddChild(new Condition("Condition 2", () => { return b2; }));
            seq.AddChild(new Condition("Condition 3", () => { return b3; }));
            root.AddChild(seq);
            BehaviorTree bt = new BehaviorTree("RandomSelectorTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status NegatorAPI(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("NegatorTest")
                .Not("Negator")
                    .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Negator(Func<Status> function)
        {
            var root = new Root();
            var inverter = new Inverter("NegatorTest");
            inverter.AddChild(new FluentBehaviorTree.Action("Action", function));
            root.AddChild(inverter);
            BehaviorTree bt = new BehaviorTree("SubtreeTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status RepeaterAPI(int timesUntilSuccess, int timesToTick)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("RepeaterTest")
                .Repeat("Repeater", timesUntilSuccess)
                    .Do("Action", ReturnSUCCESS)
                .End();
            Status result = Status.ERROR;
            for (int i = 0; i < timesToTick; ++i)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status Repeater(int timesUntilSuccess, int timesToTick)
        {
            var root = new Root();
            var repeater = new Repeater("Repeater", timesUntilSuccess);
            repeater.AddChild(new FluentBehaviorTree.Action("Action", ReturnSUCCESS));
            root.AddChild(repeater);
            BehaviorTree bt = new BehaviorTree("RepeaterTest", root);

            Status result = Status.ERROR;
            for (int i = 0; i < timesToTick; ++i)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status RepeatUntilFailAPI(int timesUntilFail, int timesToTick)
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

        Status RepeatUntilFail(int timesUntilFail, int timesToTick)
        {
            int attempts = 0;
            var root = new Root();
            var repeater = new RepeatUntilFail("RepeatUntilFail");
            repeater.AddChild(new FluentBehaviorTree.Action("Action", () =>
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
            }));
            root.AddChild(repeater);
            BehaviorTree bt = new BehaviorTree("RepeatUntilFailTest", root);

            Status result = Status.ERROR;
            for (int i = 0; i < timesToTick; ++i)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status SucceederAPI(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("SucceederTest")
                .Ignore("Succeeder")
                    .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }

        Status Succeeder(Func<Status> function)
        {
            var root = new Root();
            var succeeder = new Succeeder("Succeeder");
            succeeder.AddChild(new FluentBehaviorTree.Action("Action", function));
            root.AddChild(succeeder);
            BehaviorTree bt = new BehaviorTree("SucceederTest", root);

            Status result = bt.Tick();

            return result;
        }

        Status TimerAPI(ulong timeUntilSuccess, int waitTime)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("TimerTest")
                .Wait("Timer", timeUntilSuccess)
                    .Do("Action", ReturnSUCCESS)
                .End();
            Status result = bt.Tick();
            // Sleep for 50 ms longer, just to make sure the task finishes
            System.Threading.Thread.Sleep(waitTime + 50);
            result = bt.Tick();

            return result;
        }

        Status Timer(ulong timeUntilSuccess, int waitTime)
        {
            var root = new Root();
            var inverter = new Timer("Timer", timeUntilSuccess);
            inverter.AddChild(new FluentBehaviorTree.Action("Action", ReturnSUCCESS));
            root.AddChild(inverter);
            BehaviorTree bt = new BehaviorTree("TimerTest", root);

            Status result = bt.Tick();
            // Sleep for 50 ms longer, just to make sure the task finishes
            System.Threading.Thread.Sleep(waitTime + 50);
            result = bt.Tick();

            return result;
        }
    }
}
