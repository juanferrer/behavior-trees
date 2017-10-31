using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public abstract class Node
    {
        private string name;

        public string GetName() { return name; }
        protected void SetName(string newName) { name = newName; }

        public abstract Status Tick();
    }
}
