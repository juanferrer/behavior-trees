using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public abstract class Decorator : Branch
    {
        public Node child;

        public override void AddChild(Node n)
        {
            child = n;
        }
    }
}
