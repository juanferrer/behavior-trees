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

        public override object Clone()
        {
            Root newNode = new Root();
            newNode.AddChild((this.child).Clone() as Node);
            return newNode;
        }

        protected override Status tick()
        {
            this.Result = child.Tick();
            return this.Result;
        }
    }
}
