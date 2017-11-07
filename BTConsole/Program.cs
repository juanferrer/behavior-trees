using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentBehaviorTree;

namespace BTConsole
{
    class Program
    {
        static bool isWayBlocked = false;
        static bool isDoorLocked = true;
        static bool haveKey = false;
        static int STR = 13;
        static int DC = 12;
        static bool isDoorBroken = false;
        static bool isWindowLocked = true;
        static int ticked = 0;

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

        static public Status GoToWindow()
        {
            if (isWayBlocked)
            {
                Console.WriteLine("The way is blocked");
                return Status.FAILURE;
            }
            else if (ticked < 9)
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

        static void Main(string[] args)
        {


            BehaviorTree tree = new BehaviorTreeBuilder("Enter room")
                .RepeatUntilFail("Base loop")
                    .Selector("Find an entrance")
                        .Sequence("Try door")
                            .Not("Way is not blocked")
                                .If("Is way blocked?", IsWayBlocked)
                            .Do("Go to door", GoToDoor)
                            .Selector("Open door selector")
                                .Do("Open door", OpenDoor)
                                .Do("Unlock door", UnlockDoor)
                                .Do("Break door down", BreakDoor)
                            .End()
                            .Ignore("Try to close door")
                                .Do("Close door", CloseDoor)
                        .End()
                        .Sequence("Try window")
                            .Do("Go to window", GoToWindow)
                            .Do("Open window", OpenWindow)
                            .Do("Close window", CloseWindow)
                        .End()
                    .End();

            /*BehaviorTree tree = new BehaviorTreeBuilder("Enter room")
            .RepeatUntilFail("Root")
                .Selector("Selector")
                    .Sequence("LookNChase")
                        .If("SawPlayerInLastXSecs", () => { })
                        .Selector("Selector")
                            .Sequence("Scan for player")
                                .If("IsAlertMeterNotFull", () => { })
                                .Do("LookToLastSeenPlayerPos", () => { })
                            .End()
                            .Sequence("ChasePlayerSequence")
                                .Do("GoToLastSeenPlayerPos", () => { })
                                .If("ReachedLastSeenPlayerPos", () => { })
                                .Do("LookAroundLastSeenPlayerPos", () => { })
                            .End()
                            .Do("StillChasingPlayer", () => { })
                        .End()
                        .Sequence("Search")
                            .If("GuardSearchActive", () => { })
                            .Sequence("CheckSearchPointSequence")
                                .Do("WalkToSearchPoint", () => { })
                                .Do("WaitTillSearchPoint", () => { })
                                .Do("SweepAndResetAction", () => { })
                            .End()
                            .Sequence("Patrol")
                                .If("HasReachedWaypoint", () => { })
                                .If("HasPausedXSecs", () => { })
                                .If("HasLooked", () => { })
                                .If("HasPausedXSecsAgain", () => { })
                                .Do("ChangeWaypoint", () => { })
                                .Do("LookToNextWayPoint", () => { })
                                .Do("MoveToNextWaypoint", () => { })
                            .End()
                        .End()
                    .End()
                .End();*/


            tree.Tick();

            Console.ReadKey();
        }
    }
}
