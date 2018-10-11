using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    [Serializable]
    public class Root : Branch
    {
        public Node child;

        public Root()
        {
            Name = "Root";
        }

        public override void AddChild(Node n)
        {
            child = n;
        }

        protected override Status tick()
        {
            this.Result = child.Tick();
            return this.Result;
        }
    }
}
