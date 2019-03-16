using System;
using System.Collections.Generic;
using FluentBehaviorTree;

namespace BTTests
{
    public class APITest
    {
        [Fact]
        public void ActionSuccess()
        {
            Assert.Equal(Status.SUCCESS, Action(() => { return Status.SUCCESS; }));
        }

        Status Action(Func<Status> function)
        {
            BehaviorTree bt = new BehaviorTreeBuilder("ActionTest")
                .Do("Action", function)
                .End();

            Status result = bt.Tick();

            return result;
        }
    }
}
