using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentBehaviorTree;
using FluentBehaviorTree.Utilities;


namespace BTTests
{
    [TestClass]
    public class BTMLTests
    {
        static BehaviorTree tryDoorSequence = new BehaviorTreeBuilder("Open door")
            .Sequence("Try door")
                .Not("Way is not blocked")
                    .If("Is way blocked?", IsWayBlocked)
                .Do("Go to door", GoToDoor)
                .Selector("Open door selector")
                    .Do("Open door", OpenDoor)
                    .Do("Unlock door", UnlockDoor)
                    .Do("Break door down", BreakDoor)
                    .Sequence("Check if anyone comes")
                        .Wait("Wait for people to come", 5000)
                            .If("Someone came", SomeoneCame)
                        .Do("Ask them to open door", AskToOpenDoor)
                        .End()
                    .End()
                .Ignore("Try to close door")
                    .Do("Close door", CloseDoor)
                .End()
            .End();

        static BehaviorTree tryWindowSequence = new BehaviorTreeBuilder("OpenWindow")
            .Sequence("Try window")
                .Do("Go to window", GoToWindow)
                .Do("Open window", OpenWindow)
                .Do("Close window", CloseWindow)
                .End()
            .End();

        static BehaviorTree nestedTree = new BehaviorTreeBuilder("Nested tree")
            .Selector("Find an entrance")
                .Do("Try door", tryDoorSequence)
                .Do("Try window", tryWindowSequence)
                .End()
            .End();

        static BehaviorTree singleTree = new BehaviorTreeBuilder("Single tree")
            .Selector("Find an entrance")
                .Sequence("Try door")
                    .Not("Way is not blocked")
                        .If("Is way blocked?", IsWayBlocked)
                    .Do("Go to door", GoToDoor)
                    .Selector("Open door selector")
                        .Do("Open door", OpenDoor)
                        .Do("Unlock door", UnlockDoor)
                        .Do("Break door down", BreakDoor)
                        .Sequence("Check if anyone comes")
                            .Wait("Wait for people to come", 5000)
                                .If("Someone came", SomeoneCame)
                            .Do("Ask them to open door", AskToOpenDoor)
                            .End()
                        .End()
                    .Ignore("Try to close door")
                        .Do("Close door", CloseDoor)
                    .End()
                .Sequence("Try window")
                    .Do("Go to window", GoToWindow)
                    .Do("Open window", OpenWindow)
                    .Do("Close window", CloseWindow)
                    .End()
                .End()
            .End();

        static BehaviorTree randomTree = new BehaviorTreeBuilder("Random tree")
            .RandomSequence("Do three actions")
                .Do("Action 1", DoAction1)
                .Do("Action 2", DoAction2)
                .Do("Action 3", DoAction3)
                .End()
            .End();

        [TestMethod]
        public void SingleTreeTest()
        {
            // Way is blocked, window is locked
            ticked = 0;
            isWayBlocked = true;
            isWindowLocked = true;

            Assert.AreEqual(Status.FAILURE, RunTree(singleTree));

            // Way is blocked, window is open
            ticked = 0;
            isWayBlocked = true;
            isWindowLocked = false;

            Assert.AreEqual(Status.SUCCESS, RunTree(singleTree));

            // Way is not blocked, door is open
            ticked = 0;
            isWayBlocked = false;
            isDoorLocked = false;

            Assert.AreEqual(Status.SUCCESS, RunTree(singleTree));

            // Way is not blocked, door is locked but have key
            ticked = 0;
            isWayBlocked = false;
            isDoorLocked = true;
            haveKey = true;

            Assert.AreEqual(Status.SUCCESS, RunTree(singleTree));

            // Way is not blocked, door is locked and doesn't have key
            // Strong enough to break door
            ticked = 0;
            isWayBlocked = false;
            isDoorLocked = true;
            haveKey = false;
            STR = 15;
            DC = 15;

            Assert.AreEqual(Status.SUCCESS, RunTree(singleTree));

            // Way is not blocked, door is locked and doesn't have key
            // Too weak to break door. Someone coming
            ticked = 0;
            isWayBlocked = true;
            isDoorLocked = true;
            haveKey = false;
            STR = 10;
            DC = 15;
            someoneCame = true;

            Assert.AreEqual(Status.SUCCESS, RunTree(singleTree));

            // Way is not blocked, door is locked and doesn't have key
            // Too weak to break door. No one coming. Window locked
            ticked = 0;
            isWayBlocked = true;
            isDoorLocked = true;
            haveKey = false;
            STR = 10;
            DC = 15;
            someoneCame = false;
            isWindowLocked = true;

            Assert.AreEqual(Status.FAILURE, RunTree(singleTree));
        }

        [TestMethod]
        public void NestedTreeTest()
        {
            // Way is blocked, window is locked
            ticked = 0;
            isWayBlocked = true;
            isWindowLocked = true;

            Assert.AreEqual(Status.FAILURE, RunTree(nestedTree));

            // Way is blocked, window is open
            ticked = 0;
            isWayBlocked = true;
            isWindowLocked = false;

            Assert.AreEqual(Status.SUCCESS, RunTree(nestedTree));

            // Way is not blocked, door is open
            ticked = 0;
            isWayBlocked = false;
            isDoorLocked = false;

            Assert.AreEqual(Status.SUCCESS, RunTree(nestedTree));

            // Way is not blocked, door is locked but have key
            ticked = 0;
            isWayBlocked = false;
            isDoorLocked = true;
            haveKey = true;

            Assert.AreEqual(Status.SUCCESS, RunTree(nestedTree));

            // Way is not blocked, door is locked and doesn't have key
            // Strong enough to break door
            ticked = 0;
            isWayBlocked = false;
            isDoorLocked = true;
            haveKey = false;
            STR = 15;
            DC = 15;

            Assert.AreEqual(Status.SUCCESS, RunTree(nestedTree));

            // Way is not blocked, door is locked and doesn't have key
            // Too weak to break door. Someone coming
            ticked = 0;
            isWayBlocked = true;
            isDoorLocked = true;
            haveKey = false;
            STR = 10;
            DC = 15;
            someoneCame = true;

            Assert.AreEqual(Status.SUCCESS, RunTree(nestedTree));

            // Way is not blocked, door is locked and doesn't have key
            // Too weak to break door. No one coming. Window locked
            ticked = 0;
            isWayBlocked = true;
            isDoorLocked = true;
            haveKey = false;
            STR = 10;
            DC = 15;
            someoneCame = false;
            isWindowLocked = true;

            Assert.AreEqual(Status.FAILURE, RunTree(nestedTree));
        }

        [TestMethod]
        public void RandomTreeTest()
        {
            RandomSystem.Seed(rngSeed);
            // For the next five shuffles, list order should be:
            // 2, 1, 3
            // 3, 2, 1
            // 2, 3, 1
            // 1, 2, 3
            // 3, 2, 1

            condition1 = true;
            condition2 = true;
            condition3 = true;
            resultString = "";
            Assert.AreEqual(Status.SUCCESS, RunTree(randomTree)); // true, true, true
            Assert.AreEqual("213", resultString);

            condition2 = false;
            resultString = "";
            Assert.AreEqual(Status.FAILURE, RunTree(randomTree)); // true, false, true
            Assert.AreEqual("32", resultString);

            resultString = "";
            Assert.AreEqual(Status.FAILURE, RunTree(randomTree)); // true, false, true
            Assert.AreEqual("2", resultString);

            resultString = "";
            Assert.AreEqual(Status.FAILURE, RunTree(randomTree)); // true, false, true
            Assert.AreEqual("12", resultString);

            condition1 = false;
            condition2 = true;
            resultString = "";
            Assert.AreEqual(Status.FAILURE, RunTree(randomTree)); // false, true, true
            Assert.AreEqual("321", resultString);
        }

        public Status RunTree(BehaviorTree tree)
        {
            Status result = Status.ERROR;
            do
            {
                result = tree.Tick();
            } while (result != Status.SUCCESS && result != Status.FAILURE);
            return result;
        }

        // Helper
        static bool isWayBlocked;
        static bool isDoorLocked;
        static bool haveKey;
        static int STR;
        static int DC;
        static bool isDoorBroken;
        static bool someoneCame;
        static bool isWindowLocked;
        static int ticked;
        static bool condition1;
        static bool condition2;
        static bool condition3;
        static string resultString;
        static int rngSeed = 42;

        static public bool IsWayBlocked()
        {
            return isWayBlocked;
        }

        static public Status GoToDoor()
        {
            if (ticked < 5)
            {
                Console.WriteLine("I'm on my way");
                ticked++;
                return Status.RUNNING;
            }
            else
            {
                Console.WriteLine("I'm at the door");
                return Status.SUCCESS;
            }
        }

        static public Status OpenDoor()
        {
            if (isDoorLocked)
            {
                Console.WriteLine("Door is locked");
                return Status.FAILURE;
            }
            else
            {
                Console.WriteLine("Opening door");
                return Status.SUCCESS;
            }
        }

        static public Status UnlockDoor()
        {
            if (!haveKey)
            {
                Console.WriteLine("Can't unlock the door");
                return Status.FAILURE;
            }
            else
            {
                Console.WriteLine("Door unlocked");
                return Status.SUCCESS;
            }
        }

        static public Status BreakDoor()
        {
            if (DC > STR)
            {
                Console.WriteLine("Door is too strong");
                return Status.FAILURE;
            }
            else
            {
                Console.WriteLine("Breaking door");
                isDoorBroken = true;
                return Status.SUCCESS;
            }
        }

        static public bool IsDoorBroken()
        {
            return isDoorBroken;
        }

        static public Status CloseDoor()
        {
            if (!isDoorBroken)
            {
                Console.WriteLine("Closing door");
                return Status.SUCCESS;
            }
            else
            {
                Console.WriteLine("Can't close a broken door");
                return Status.FAILURE;
            }
        }

        static public bool SomeoneCame()
        {
            if (someoneCame)
            {
                Console.WriteLine("Hey, someone's here!");
                return true;
            }
            else
            {
                Console.WriteLine("No one seems to be coming");
                return false;
            }
        }

        static public Status AskToOpenDoor()
        {
            Console.WriteLine("Person opened the door");
            return Status.SUCCESS;
        }

        static public Status GoToWindow()
        {
            if (ticked < 9)
            {
                Console.WriteLine("I'm on my way");
                ticked++;
                return Status.RUNNING;
            }
            else
            {
                Console.WriteLine("I'm at the window");
                return Status.SUCCESS;
            }
        }

        static public Status OpenWindow()
        {
            if (isWindowLocked)
            {
                Console.WriteLine("Window is locked");
                return Status.FAILURE;
            }
            else
            {
                Console.WriteLine("Opening window");
                return Status.SUCCESS;
            }
        }

        static public Status CloseWindow()
        {
            Console.WriteLine("Closing window");
            return Status.SUCCESS;
        }

        static public Status DoAction1()
        {
            Console.WriteLine("Performing Action1");
            resultString += "1";
            if (condition1) return Status.SUCCESS;
            else return Status.FAILURE;
        }

        static public Status DoAction2()
        {
            Console.WriteLine("Performing Action2");
            resultString += "2";
            if (condition2) return Status.SUCCESS;
            else return Status.FAILURE;
        }


        static public Status DoAction3()
        {
            Console.WriteLine("Performing Action3");
            resultString += "3";
            if (condition3) return Status.SUCCESS;
            else return Status.FAILURE;
        }
    }
}
