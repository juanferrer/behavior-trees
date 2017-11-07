using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public abstract class Composite : Branch
    {
        public List<Node> children;

        public override void AddChild(Node n)
        {
            children.Add(n);
        }
    }
}
