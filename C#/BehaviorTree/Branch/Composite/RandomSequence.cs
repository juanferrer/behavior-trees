using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class RandomSequence : Sequence
    {
        public RandomSequence(string name) : base(name)
        { }

        protected override void Open()
        {
            base.Open();

            children.Shuffle();
        }
    }
}
