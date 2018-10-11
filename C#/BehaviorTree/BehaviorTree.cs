using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class BehaviorTree
    {
        private string name;

        public string GetName() { return name; }
        public Node GetRoot() { return root; }
        protected void SetName(string newName) { name = newName; }


        private Node root;

        public BehaviorTree(string newName, Node newRoot)
        {
            name = newName;
            root = newRoot;
        }

        /// <summary>
        /// Tick tree to navigate and get result
        /// </summary>
        /// <returns></returns>
        public Status Tick()
        {
            var status = root.Tick();
            return status;
        }
    }
}
