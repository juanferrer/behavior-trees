using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public abstract class Composite : Branch
    {
        protected List<Node> children;

        public override void AddChild(Node n)
        {
            children.Add(n);
        }

        protected override void Open()
        {
            base.Open();

            foreach (var child in children)
            {
                child.Result = Status.RUNNING;
            }
        }
    }
}
