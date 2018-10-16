using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public abstract class Node
    {
        public string Name { get; protected set; }
        public bool IsOpen { get; protected set; }
        public Status Result { get; set; }

        public abstract Node Copy();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Status Tick()
        {
            Enter();

            if (!IsOpen) Open();

            var status = tick();

            Exit();
            if (status == Status.RUNNING) return status;

            Close();
            return status;
        }

        /// <summary>
        /// Enter node and prepare for execution
        /// </summary>
        private void Enter()
        {
            //
        }

        /// <summary>
        /// Open node only if node has not been opened before
        /// </summary>
        protected virtual void Open()
        {
            IsOpen = true;

            this.Result = Status.RUNNING;
        }

        /// <summary>
        /// Actual tick to be overriden by every child
        /// </summary>
        protected abstract Status tick();

        /// <summary>
        /// Exit node after every tick
        /// </summary>
        private void Exit()
        {
            //
        }

        /// <summary>
        /// Close node to ensure we don't go through it again
        /// </summary>
        private void Close()
        {
            IsOpen = false;
        }

    }
}
