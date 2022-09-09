using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBehaviorTree;
using FluentBehaviorTree.Utilities;
using System.Runtime.CompilerServices;

namespace BTTests
{
    [TestClass]
    public class APITests
    {
        public TestContext TestContext { get; set; }
        private int leafExecuted;
        private int rngSeed = 42;

        private const int N = 3;
        private int successN = 0;
        private int failureN = 0;
        private int errorN = 0;

        private void ResetN()
        {
            successN = 0;
            failureN = 0;
            errorN = 0;
        }

        // Leaf nodes
        [TestMethod]
        public void ActionTest()
        {
            Assert.AreEqual(Status.SUCCESS, Action(ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Action(ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Action(ReturnRUNNING));
            Assert.AreEqual(Status.ERROR,   Action(ReturnERROR));
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
            Assert.AreEqual(Status.SUCCESS, Subtree(ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Subtree(ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Subtree(ReturnRUNNING));
            Assert.AreEqual(Status.ERROR,   Subtree(ReturnERROR));
        }

        // Composites
        [TestMethod]
        public void SequenceBoolTest()
        {
            // Only succeeds when all are true
            Assert.AreEqual(Status.SUCCESS, Sequence(true,  true,  true));
            Assert.AreEqual(Status.FAILURE, Sequence(true,  true,  false));
            Assert.AreEqual(Status.FAILURE, Sequence(true,  false, false));
            Assert.AreEqual(Status.FAILURE, Sequence(false, false, false));
            Assert.AreEqual(Status.FAILURE, Sequence(false, true,  true));
            Assert.AreEqual(Status.FAILURE, Sequence(false, true,  false));
            Assert.AreEqual(Status.FAILURE, Sequence(true,  false, true));
            Assert.AreEqual(Status.FAILURE, Sequence(false, false, true));

        }

        [TestMethod]
        public void SequenceActionTest()
        { 
            // Only succeeds when all are SUCCESS
            Assert.AreEqual(Status.SUCCESS, Sequence(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnSUCCESS, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnSUCCESS, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnSUCCESS, ReturnFAILURE, ReturnFAILURE));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnSUCCESS, ReturnFAILURE, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnRUNNING, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnRUNNING, ReturnRUNNING));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnFAILURE, ReturnFAILURE));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnFAILURE, ReturnRUNNING));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnRUNNING, ReturnFAILURE));
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILURE, ReturnRUNNING, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnFAILURE, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnFAILURE, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnRUNNING, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnRUNNING, ReturnRUNNING, ReturnRUNNING));
        }

        [TestMethod]
        public void SequenceRepeatedActionTest()
        {
            // Fails after ticking through 1
            ResetN();
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N));
            Assert.AreEqual(1, leafExecuted);

            // Doesn't finish ticking 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N - 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1, then ticks 1 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N + 1));
            Assert.AreEqual(1, leafExecuted);

            // Get's a failure after it's done ticking 1 for the second time
            ResetN();
            Assert.AreEqual(Status.FAILURE, Sequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N * 2));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1 then gets stuck in 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, (N * 2) - 1));
            Assert.AreEqual(1, leafExecuted);

            //-------------------------------------------------------------------------------------------------//

            // Ticks through 1, 2 and 3
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnSUCCESS, N));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1 and doesn't finish ticking 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnSUCCESS, N - 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and 3 and then ticks 1 and 2 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnSUCCESS, N + 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and 3 twice
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnSUCCESS, N * 2));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1, 2 and 3, then ticks 1 and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnSUCCESS, (N * 2) - 1));
            Assert.AreEqual(2, leafExecuted);

            //-------------------------------------------------------------------------------------------------//

            // Ticks through 1, 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnFAILUREOnN, N));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1 and doesn't finish ticking 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnFAILUREOnN, N - 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and ticks 3 twice
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnFAILUREOnN, N + 1));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1, 2 and 3, and then ticks 1 and 2 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESS, ReturnSUCCESSOnN, ReturnFAILUREOnN, N * 2));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESSOnN, ReturnSUCCESSOnN, ReturnFAILUREOnN, (N * 2) - 1));
            Assert.AreEqual(3, leafExecuted);

            //-------------------------------------------------------------------------------------------------//

            // Ticks through 1 and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESSOnN, ReturnSUCCESSOnN, ReturnSUCCESS, N));
            Assert.AreEqual(2, leafExecuted);

            // Doesn't finish ticking 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESSOnN, ReturnSUCCESSOnN, ReturnSUCCESS, N - 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1, and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESSOnN, ReturnSUCCESSOnN, ReturnSUCCESS, N + 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and 3, and then ticks 1 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Sequence(ReturnSUCCESSOnN, ReturnSUCCESSOnN, ReturnSUCCESS, N * 2));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1, 2 and 3
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Sequence(ReturnSUCCESSOnN, ReturnSUCCESSOnN, ReturnSUCCESS, (N * 2) - 1));
            Assert.AreEqual(3, leafExecuted);
        }

        [TestMethod]
        public void SelectorBoolTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Status.SUCCESS, Selector(true,  true,  true));
            Assert.AreEqual(Status.SUCCESS, Selector(true,  true,  false));
            Assert.AreEqual(Status.SUCCESS, Selector(true,  false, false));
            Assert.AreEqual(Status.FAILURE, Selector(false, false, false));
            Assert.AreEqual(Status.SUCCESS, Selector(false, true,  true));
            Assert.AreEqual(Status.SUCCESS, Selector(false, true,  false));
            Assert.AreEqual(Status.SUCCESS, Selector(true,  false, true));
            Assert.AreEqual(Status.SUCCESS, Selector(false, false, true));
        }

        [TestMethod]
        public void SelectorActionTest()
        { 
            // Only fails when all are FAILURE
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnFAILURE, ReturnFAILURE));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnFAILURE, ReturnRUNNING));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnRUNNING, ReturnFAILURE));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnSUCCESS, ReturnRUNNING, ReturnRUNNING));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILURE, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILURE, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILURE, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual(Status.FAILURE, Selector(ReturnFAILURE, ReturnFAILURE, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnFAILURE, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnRUNNING, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnRUNNING, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnFAILURE, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnFAILURE, ReturnRUNNING));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnRUNNING, ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Selector(ReturnRUNNING, ReturnRUNNING, ReturnRUNNING));
        }

        [TestMethod]
        public void SelectorRepeatedActionTest()
        {
            // Ticks through 1 and 2
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N));
            Assert.AreEqual(2, leafExecuted);

            // Doesn't finish ticking 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N - 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1 and 2, then ticks 1 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N + 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1 and 2 twice
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N * 2));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1 and 2 then gets stuck in 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, (N * 2) - 1));
            Assert.AreEqual(1, leafExecuted);

            //-------------------------------------------------------------------------------------------------//

            // Ticks through 1 and 2
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILURE, ReturnSUCCESSOnN, ReturnSUCCESS, N));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1 and doesn't finish ticking 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnSUCCESSOnN, ReturnSUCCESS, N - 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2, then ticks 1 and 2 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnSUCCESSOnN, ReturnSUCCESS, N + 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 twice
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILURE, ReturnSUCCESSOnN, ReturnSUCCESS, N * 2));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2, then ticks 1 and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnSUCCESSOnN, ReturnSUCCESS, (N * 2) - 1));
            Assert.AreEqual(2, leafExecuted);

            //-------------------------------------------------------------------------------------------------//

            // Ticks through 1, 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnFAILUREOnN, ReturnFAILUREOnN, N));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1 and doesn't finish ticking 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnFAILUREOnN, ReturnFAILUREOnN, N - 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnFAILUREOnN, ReturnFAILUREOnN, N + 1));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1, 2 and 3, and then ticks 1 and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILURE, ReturnFAILUREOnN, ReturnFAILUREOnN, N * 2));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and 3
            ResetN();
            Assert.AreEqual(Status.FAILURE, Selector(ReturnFAILURE, ReturnFAILUREOnN, ReturnFAILUREOnN, (N * 2) - 1));
            Assert.AreEqual(3, leafExecuted);

            //-------------------------------------------------------------------------------------------------//

            // Ticks through 1 and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnFAILUREOnN, ReturnSUCCESS, N));
            Assert.AreEqual(2, leafExecuted);

            // Doesn't finish ticking 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnFAILUREOnN, ReturnSUCCESS, N - 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1, and gets stuck in 2
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnFAILUREOnN, ReturnSUCCESS, N + 1));
            Assert.AreEqual(2, leafExecuted);

            // Ticks through 1, 2 and 3, and then ticks 1 once
            ResetN();
            Assert.AreEqual(Status.RUNNING, Selector(ReturnFAILUREOnN, ReturnFAILUREOnN, ReturnSUCCESS, N * 2));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 1, 2 and 3
            ResetN();
            Assert.AreEqual(Status.SUCCESS, Selector(ReturnFAILUREOnN, ReturnFAILUREOnN, ReturnSUCCESS, (N * 2) - 1));
            Assert.AreEqual(3, leafExecuted);
        }

        [TestMethod]
        public void RandomSequenceBoolTest()
        {
            // Only succeeds when all are true
            Assert.AreEqual(Status.SUCCESS, RandomSequence(true, true, true));
            Assert.AreEqual(Status.FAILURE, RandomSequence(true, true, false));
            Assert.AreEqual(Status.FAILURE, RandomSequence(true, false, false));
            Assert.AreEqual(Status.FAILURE, RandomSequence(false, false, false));
            Assert.AreEqual(Status.FAILURE, RandomSequence(false, true, true));
            Assert.AreEqual(Status.FAILURE, RandomSequence(false, true, false));
            Assert.AreEqual(Status.FAILURE, RandomSequence(true, false, true));
            Assert.AreEqual(Status.FAILURE, RandomSequence(false, false, true));

            // Also check it's being executed at random
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1
            RandomSequence(false, true, false);
            Assert.AreEqual(1, leafExecuted);
            RandomSequence(false, true, false);
            Assert.AreEqual(3, leafExecuted);
            RandomSequence(false, true, true);
            Assert.AreEqual(1, leafExecuted);
            RandomSequence(false, false, false);
            Assert.AreEqual(1, leafExecuted);
            RandomSequence(false, false, true);
            Assert.AreEqual(2, leafExecuted);
        }

        [TestMethod]
        public void RandomSequenceActionTest()
        {
            // Only succeeds when all are SUCCESS
            Assert.AreEqual   (Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnFAILURE, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnFAILURE, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnRUNNING, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnRUNNING, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnFAILURE, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnFAILURE, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnRUNNING, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnFAILURE, ReturnRUNNING, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnFAILURE, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnFAILURE, ReturnRUNNING));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnRUNNING, ReturnFAILURE));
            Assert.AreNotEqual(Status.SUCCESS, RandomSequence(ReturnRUNNING, ReturnRUNNING, ReturnRUNNING));

            // Also check it's being executed at random
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1
            RandomSequence(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE);
            Assert.AreEqual(1, leafExecuted);
            RandomSequence(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE);
            Assert.AreEqual(3, leafExecuted);
            RandomSequence(ReturnFAILURE, ReturnSUCCESS, ReturnSUCCESS);
            Assert.AreEqual(1, leafExecuted);
            RandomSequence(ReturnFAILURE, ReturnFAILURE, ReturnFAILURE);
            Assert.AreEqual(1, leafExecuted);
            RandomSequence(ReturnFAILURE, ReturnFAILURE, ReturnSUCCESS);
            Assert.AreEqual(2, leafExecuted);
        }

        [TestMethod]
        public void RandomSequenceRepeatedActionTest()
        {
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1
            // 3, 1, 2

            // Ticks 2 and 1
            ResetN();
            Assert.AreEqual(Status.FAILURE, RandomSequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnFAILURE, N));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 3 and 2 and gets stuck in 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnSUCCESSOnN, N + 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSequence(ReturnFAILUREOnN, ReturnSUCCESS, ReturnSUCCESSOnN, N - 1));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1, 2 and 3
            ResetN();
            Assert.AreEqual(Status.SUCCESS, RandomSequence(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESSOnN, N));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 3, 2 and 1, then gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSequence(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESSOnN, N + 1));
            Assert.AreEqual(3, leafExecuted);

            // Gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSequence(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESSOnN, N - 1));
            Assert.AreEqual(3, leafExecuted);
        }              

        [TestMethod]
        public void RandomSelectorBoolTest()
        {
            Assert.AreEqual(Status.SUCCESS, RandomSelector(true, true, true));
            Assert.AreEqual(Status.SUCCESS, RandomSelector(true, true, false));
            Assert.AreEqual(Status.SUCCESS, RandomSelector(true, false, false));
            Assert.AreEqual(Status.FAILURE, RandomSelector(false, false, false));
            Assert.AreEqual(Status.SUCCESS, RandomSelector(false, true, true));
            Assert.AreEqual(Status.SUCCESS, RandomSelector(false, true, false));
            Assert.AreEqual(Status.SUCCESS, RandomSelector(true, false, true));
            Assert.AreEqual(Status.SUCCESS, RandomSelector(false, false, true));

            // Also check it's being executed at random
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1
            RandomSelector(false, true, false);
            Assert.AreEqual(2, leafExecuted);
            RandomSelector(false, true, false);
            Assert.AreEqual(2, leafExecuted);
            RandomSelector(false, true, true);
            Assert.AreEqual(2, leafExecuted);
            RandomSelector(false, false, true);
            Assert.AreEqual(3, leafExecuted);
            RandomSelector(false, false, true);
            Assert.AreEqual(3, leafExecuted);
        }

        [TestMethod]
        public void RandomSelectorActionTest()
        {
            // Only succeeds when all are SUCCESS
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnFAILURE, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnFAILURE, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnRUNNING, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnSUCCESS, ReturnRUNNING, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreEqual   (Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnFAILURE, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnFAILURE, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnRUNNING, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnFAILURE, ReturnRUNNING, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnSUCCESS, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnSUCCESS, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnSUCCESS, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnFAILURE, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnFAILURE, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnFAILURE, ReturnRUNNING));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnRUNNING, ReturnSUCCESS));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnRUNNING, ReturnFAILURE));
            Assert.AreNotEqual(Status.FAILURE, RandomSelector(ReturnRUNNING, ReturnRUNNING, ReturnRUNNING));

            // Also check it's being executed at random
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1
            RandomSelector(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE);
            Assert.AreEqual(2, leafExecuted);
            RandomSelector(ReturnFAILURE, ReturnSUCCESS, ReturnFAILURE);
            Assert.AreEqual(2, leafExecuted);
            RandomSelector(ReturnFAILURE, ReturnSUCCESS, ReturnSUCCESS);
            Assert.AreEqual(2, leafExecuted);
            RandomSelector(ReturnFAILURE, ReturnFAILURE, ReturnFAILURE);
            Assert.AreEqual(3, leafExecuted);
            RandomSelector(ReturnFAILURE, ReturnFAILURE, ReturnSUCCESS);
            Assert.AreEqual(3, leafExecuted);
        }

        [TestMethod]
        public void RandomSelectorRepeatedActionTest()
        {
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1
            // 3, 1, 2

            // Ticks 2
            ResetN();
            Assert.AreEqual(Status.SUCCESS, RandomSelector(ReturnSUCCESSOnN, ReturnFAILURE, ReturnFAILUREOnN, N));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 3 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSelector(ReturnSUCCESSOnN, ReturnFAILURE, ReturnFAILUREOnN, N + 1));
            Assert.AreEqual(1, leafExecuted);

            // Ticks through 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSelector(ReturnSUCCESSOnN, ReturnFAILURE, ReturnFAILUREOnN, N - 1));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 1 and 2 and gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSelector(ReturnFAILUREOnN, ReturnFAILURE, ReturnFAILUREOnN, N));
            Assert.AreEqual(3, leafExecuted);

            // Ticks through 3, 2 and then gets stuck in 1
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSelector(ReturnFAILUREOnN, ReturnFAILURE, ReturnFAILUREOnN, N + 1));
            Assert.AreEqual(1, leafExecuted);

            // Gets stuck in 3
            ResetN();
            Assert.AreEqual(Status.RUNNING, RandomSelector(ReturnFAILUREOnN, ReturnFAILURE, ReturnFAILUREOnN, N - 1));
            Assert.AreEqual(3, leafExecuted);
        }

        // Decorators
        [TestMethod]
        public void NegatorTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Status.FAILURE, Negator(ReturnSUCCESS));
            Assert.AreEqual(Status.SUCCESS, Negator(ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Negator(ReturnRUNNING));
            Assert.AreEqual(Status.ERROR,   Negator(ReturnERROR));
        }

        [TestMethod]
        public void RepeaterTest()
        {
            Assert.AreEqual(Status.SUCCESS, Repeater(5, 5));
            Assert.AreEqual(Status.SUCCESS, Repeater(2, 5));
            Assert.AreEqual(Status.SUCCESS, Repeater(0, 5));
            Assert.AreEqual(Status.RUNNING, Repeater(5, 1));
            Assert.AreEqual(Status.ERROR,   Repeater(0, 0));
        }

        [TestMethod]
        public void RepeatUntilFailTest()
        {
            Assert.AreEqual(Status.SUCCESS, RepeatUntilFail(5, 5));
            Assert.AreEqual(Status.SUCCESS, RepeatUntilFail(2, 5));
            Assert.AreEqual(Status.SUCCESS, RepeatUntilFail(0, 5));
            Assert.AreEqual(Status.RUNNING, RepeatUntilFail(5, 1));
            Assert.AreEqual(Status.ERROR,   RepeatUntilFail(0, 0));
        }

        [TestMethod]
        public void SucceederTest()
        {
            // Only fails when all are false
            Assert.AreEqual(Status.SUCCESS, Succeeder(ReturnSUCCESS));
            Assert.AreEqual(Status.SUCCESS, Succeeder(ReturnFAILURE));
            Assert.AreEqual(Status.RUNNING, Succeeder(ReturnRUNNING));
            Assert.AreEqual(Status.ERROR,   Succeeder(ReturnERROR));
        }

        [TestMethod]
        public void TimerTest()
        {
            Assert.AreEqual(Status.SUCCESS, Timer(100, 100));
            Assert.AreEqual(Status.RUNNING, Timer(300, 100));
            Assert.AreEqual(Status.SUCCESS, Timer(100, 300));
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

        Status ReturnSUCCESSOnN()
        {
            successN++;
            if (successN % N != 0)
            {
                return Status.RUNNING;
            }

            return Status.SUCCESS;
        }

        Status ReturnFAILUREOnN()
        {
            failureN++;
            if (failureN % N != 0)
            {
                return Status.RUNNING;
            }

            return Status.FAILURE;
        }

        Status ReturnERROROnN()
        {
            errorN++;
            if (errorN % N != 0)
            {
                return Status.RUNNING;
            }

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

        Status Sequence(bool b1, bool b2, bool b3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("SequenceTest")
                .Sequence("Sequence")
                    .If("Condition 1", () =>
                    {
                        TestContext?.WriteLine("Condition 1: {0}", b1);
                        leafExecuted = 1;
                        return b1;
                    })
                    .If("Condition 2", () =>
                    {
                        TestContext?.WriteLine("Condition 2: {0}", b2);
                        leafExecuted = 2;
                        return b2;
                    })
                    .If("Condition 3", () =>
                    {
                        TestContext?.WriteLine("Condition 3: {0}", b3);
                        leafExecuted = 3;
                        return b3;
                    })
                    .End()
                .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();                
            }

            return result;
        }

        Status Sequence(Func<Status> a1, Func<Status> a2, Func<Status> a3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("SequenceTest")
            .Sequence("Sequence")
                .Do("Action 1", () =>
                {
                    var localResult = a1.Invoke();
                    TestContext?.WriteLine("Action 1: {0}", localResult);
                    leafExecuted = 1;
                    return localResult;
                })
                .Do("Action 2", () =>
                {
                    var localResult = a2.Invoke();
                    TestContext?.WriteLine("Action 2: {0}", localResult);
                    leafExecuted = 2;
                    return localResult;
                })
                .Do("Action 3", () =>
                {
                    var localResult = a3.Invoke();
                    TestContext?.WriteLine("Action 3: {0}", localResult);
                    leafExecuted = 3;
                    return localResult;
                })
                .End()
            .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status Selector(bool b1, bool b2, bool b3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("SelectorTest")
                .Selector("Selector")
                    .If("Condition 1", () =>
                    {
                        TestContext?.WriteLine("Condition 1: {0}", b1);
                        leafExecuted = 1;
                        return b1;
                    })
                    .If("Condition 2", () =>
                    {
                        TestContext?.WriteLine("Condition 2: {0}", b2);
                        leafExecuted = 2;
                        return b2;
                    })
                    .If("Condition 3", () =>
                    {
                        TestContext?.WriteLine("Condition 3: {0}", b3);
                        leafExecuted = 3;
                        return b3;
                    })
                    .End()
                .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();                
            }

            return result;
        }

        Status Selector(Func<Status> a1, Func<Status> a2, Func<Status> a3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("SelectorTest")
            .Selector("Sequence")
                .Do("Action 1", () =>
                {
                    var localResult = a1.Invoke();
                    TestContext?.WriteLine("Action 1: {0}", localResult);
                    leafExecuted = 1;
                    return localResult;
                })
                .Do("Action 2", () =>
                {
                    var localResult = a2.Invoke();
                    TestContext?.WriteLine("Action 2: {0}", localResult);
                    leafExecuted = 2;
                    return localResult;
                })
                .Do("Action 3", () =>
                {
                    var localResult = a3.Invoke();
                    TestContext?.WriteLine("Action 3: {0}", localResult);
                    leafExecuted = 3;
                    return localResult;
                })
                .End()
            .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();                
            }

            return result;
        }

        Status RandomSequence(bool b1, bool b2, bool b3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("RandomSequenceTest")
                .RandomSequence("RandomSequence")
                    .If("Condition 1", () =>
                    {
                        TestContext?.WriteLine("Condition 1: {0}", b1);
                        leafExecuted = 1;
                        return b1;
                    })
                    .If("Condition 2", () =>
                    {
                        TestContext?.WriteLine("Condition 2: {0}", b2);
                        leafExecuted = 2;
                        return b2;
                    })
                    .If("Condition 3", () =>
                    {
                        TestContext?.WriteLine("Condition 3: {0}", b3);
                        leafExecuted = 3;
                        return b3;
                    })
                    .End()
                .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();                
            }

            return result;
        }

        Status RandomSequence(Func<Status> a1, Func<Status> a2, Func<Status> a3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("RandomSequenceTest")
                .RandomSequence("RandomSequence")
                    .Do("Action 1", () =>
                    {
                        var localResult = a1.Invoke();
                        TestContext?.WriteLine("Action 1: {0}", localResult);
                        leafExecuted = 1;
                        return localResult;
                    })
                    .Do("Action 2", () =>
                    {
                        var localResult = a2.Invoke();
                        TestContext?.WriteLine("Action 2: {0}", localResult);
                        leafExecuted = 2;
                        return localResult;
                    })
                    .Do("Action 3", () =>
                    {
                        var localResult = a3.Invoke();
                        TestContext?.WriteLine("Action 3: {0}", localResult);
                        leafExecuted = 3;
                        return localResult;
                    })
                    .End()
                .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();
            }

            return result;
        }

        Status RandomSelector(bool b1, bool b2, bool b3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("RandomSelectorTest")
                .RandomSelector("Random selector")
                    .If("Condition 1", () =>
                    {
                        TestContext?.WriteLine("Condition 1: {0}", b1);
                        leafExecuted = 1;
                        return b1;
                    })
                    .If("Condition 2", () =>
                    {
                        TestContext?.WriteLine("Condition 2: {0}", b2);
                        leafExecuted = 2;
                        return b2;
                    })
                    .If("Condition 3", () =>
                    {
                        TestContext?.WriteLine("Condition 3: {0}", b3);
                        leafExecuted = 3;
                        return b3;
                    })
                    .End()
                .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();                
            }

            return result;
        }

        Status RandomSelector(Func<Status> a1, Func<Status> a2, Func<Status> a3, int tickN = 1)
        {
            TestContext?.WriteLine("Test: {0}", TestContext?.TestName);
            BehaviorTree bt = new BehaviorTreeBuilder("RandomSelectorTest")
                .RandomSelector("Random selector")
                    .Do("Action 1", () =>
                    {
                        var localResult = a1.Invoke();
                        TestContext?.WriteLine("Action 1: {0}", localResult);
                        leafExecuted = 1;
                        return localResult;
                    })
                    .Do("Action 2", () =>
                    {
                        var localResult = a2.Invoke();
                        TestContext?.WriteLine("Action 2: {0}", localResult);
                        leafExecuted = 2;
                        return localResult;
                    })
                    .Do("Action 3", () =>
                    {
                        var localResult = a3.Invoke();
                        TestContext?.WriteLine("Action 3: {0}", localResult);
                        leafExecuted = 3;
                        return localResult;
                    })
                    .End()
                .End();

            Status result = Status.ERROR;
            for (int i = 0; i < tickN; i++)
            {
                result = bt.Tick();                
            }

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

        Status Repeater(int timesUntilSuccess, int tickN)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("RepeaterTest")
                .Repeat("Repeater", timesUntilSuccess)
                    .Do("Action", () =>{ return Status.SUCCESS; })
                .End();
            Status result = Status.ERROR;
            for (int i = 0; i < tickN; ++i)
            {
                result = bt.Tick();                
            }

            return result;
        }

        Status RepeatUntilFail(int timesUntilFail, int tickN)
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
            for (int i = 0; i < tickN; ++i)
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
                    .Do("Action", ReturnSUCCESS)
                .End();
            Status result = bt.Tick();
            // Sleep for 50 ms longer, just to make sure the task finishes
            System.Threading.Thread.Sleep(waitTime + 50);
            result = bt.Tick();

            return result;
        }
    }
}
