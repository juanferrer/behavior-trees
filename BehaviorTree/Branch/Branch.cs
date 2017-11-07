using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public abstract class Branch : Node
    {
        public abstract void AddChild(Node n);
    }
}
