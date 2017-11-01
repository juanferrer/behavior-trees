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
        static int STR = 10;
        static int DC = 12;
        static bool isDoorBroken = false;
        static bool isWindowLocked = true;
        static int ticked = 0;

        static public Status GoToDoor()
        {
            if (isWayBlocked)
            {
                Console.WriteLine("The way is blocked");
                return Status.FAILURE;
            }
            else if (ticked < 5)
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
            Console.WriteLine("Closing door");
            return Status.SUCCESS;
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
                .Repeat("Base loop")
                    .Selector("Find an entrance")
                        .Sequence("Try door")
                            .Do("Go to door", GoToDoor)
                            .Selector("Open door selector")
                                .Do("Open door", OpenDoor)
                                .Do("Unlock door", UnlockDoor)
                                .Do("Break door down", BreakDoor)
                            .End()
                            .Do("Close door", CloseDoor)
                        .End()
                        .Sequence("Try window")
                            .Do("Go to window", GoToWindow)
                            .Do("Open window", OpenWindow)
                            .Do("Close window", CloseWindow)
                        .End()
                    .End();

            tree.Tick();

            Console.ReadKey();
        }
    }
}
