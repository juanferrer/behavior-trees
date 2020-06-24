using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class RandomSelector : Composite
    {
        List<Node> randomisedChildren;

        public RandomSelector(string name)
        {
            this.Name = name;
            children = new List<Node>();
            randomisedChildren = new List<Node>();
        }

        public override object Clone()
        {
            RandomSelector newNode = new RandomSelector(this.Name);
            for (int i = 0, size = children.Count; i < size; ++i)
            {
                newNode.AddChild(children[i].Clone() as Node);
            }
            return newNode;
        }

        protected override void Open()
        {
            base.Open();

            // Also randomise children
            if (randomisedChildren.Count == 0)
            {
                randomisedChildren = (List<Node>)children.Clone();
                randomisedChildren.Shuffle();
            }
        }

        protected override void Close()
        {
            base.Close();
            randomisedChildren = new List<Node>();
        }

        /// <summary>
        /// Propagate tick to children. Return FAILURE if no child succeeds. Shuffle children before running
        /// </summary>
        /// <returns></returns>
        protected override Status tick()
        {
            if (this.Result == Status.RUNNING)
            {
                while (randomisedChildren.Count > 0)
                {
                    // Remove first item, which will be different on each run
                    var n = randomisedChildren[0];
                    randomisedChildren.RemoveAt(0);
                    {
                        if (!n.IsOpen && n.Result == Status.SUCCESS)
                        {
                            this.Result = Status.SUCCESS;
                            return this.Result;
                        }
                        else if (n.Result == Status.RUNNING)
                        {
                            this.Result = n.Tick();
                            if (this.Result != Status.FAILURE) return this.Result;
                        }
                    }
                }

                this.Result = Status.FAILURE;
            }
            return this.Result;
        }
    }
}
