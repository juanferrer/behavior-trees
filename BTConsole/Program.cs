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
        static void Main(string[] args)
        {
            bool isWayBlocked = false;
            bool isDoorLocked = true;
            bool isWindowLocked = false;
            int ticked = 0;

            /*var repeater = new Repeater(6);

            var root = new Sequence();

            var goToDoor = new BehaviourTrees.Action(() =>
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
            });

            var openDoor = new BehaviourTrees.Action(() =>
            {
                Console.WriteLine("Opening door");
                return isDoorLocked ? Status.FAILURE : Status.SUCCESS;
            });

            var succeeder = new Succeeder();
            var closeDoor = new BehaviourTrees.Action(() =>
            {
                Console.WriteLine("Closing door");
                return Status.SUCCESS;
            });

            repeater.AddChild(root);
            succeeder.AddChild(openDoor);

            root.AddChild(goToDoor);
            root.AddChild(succeeder);
            root.AddChild(closeDoor);

            repeater.Tick();*/

            BehaviorTree tree = new BehaviorTreeBuilder("Enter room")
                .Repeat("Base loop", 9)
                    .Selector("Find an entrance")
                        .Sequence("Try door")
                            .Do("Go to door", () =>
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
                            })
                            .Do("Open door", () =>
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
                            })
                            /*.Not("Door not broken")
                                .If("Door broken?", () =>
                                {
                                    return false;
                                })*/
                            .Do("Close door", () =>
                            {
                                Console.WriteLine("Closing door");
                                return Status.SUCCESS;
                            })
                        .End()
                        .Sequence("Try window")
                            .Do("Go to window", () =>
                            {
                                if (ticked < 8)
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
                            })
                            .Do("Open window", () =>
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
                            })
                            /*.Not("Door not broken")
                                .If("Door broken?", () =>
                                {
                                    return false;
                                })*/
                            .Do("Close window", () =>
                            {
                                Console.WriteLine("Closing window");
                                return Status.SUCCESS;
                            })
                        .End()
                    .End();

            tree.Tick();

            Console.ReadKey();
        }
    }
}
