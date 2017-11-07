﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBehaviorTree
{
    public class BehaviorTreeBuilder
    {
        private string name;
        private Stack<Node> parentNodes;

        public BehaviorTreeBuilder(string newName)
        {
            name = newName;
            parentNodes = new Stack<Node>();
        }

        /*********************/
        /******* LEAF ********/
        /*********************/

        /// <summary>
        /// Add an action to the tree
        /// </summary>
        /// <param name="name"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Do(string name, Func<Status> f)
        {
            var currentNode = new Action(f);

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            while (parentNodes.Count > 1 && parentNodes.Peek() is Decorator)
            {
                parentNodes.Pop();              // Remove decorators from parentNodes until you reach next Composite
            }

            return this;
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="name"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder If(string name, Func<bool> f)
        {
            var currentNode = new Condition(f);

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            while (parentNodes.Count > 1 && parentNodes.Peek() is Decorator)
            {
                parentNodes.Pop();              // Remove decorators from parentNodes until you reach next Composite
            }

            return this;
        }

        /*********************/
        /***** DECORATOR *****/
        /*********************/

        /// <summary>
        /// Negate child
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Not(string name)
        {
            var currentNode = new Inverter();

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            parentNodes.Push(currentNode);

            return this;
        }

        /// <summary>
        /// Repeatedly tick child n times. If n == 0, tick forever
        /// </summary>
        /// <param name="name"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Repeat(string name, int n = 0)
        {
            var currentNode = new Repeater(n);

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            parentNodes.Push(currentNode);

            return this;
        }

        /// <summary>
        /// Repeatedly tick child until FAILURE is returned
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder RepeatUntilFail(string name)
        {
            var currentNode = new RepeatUntilFail();

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            parentNodes.Push(currentNode);

            return this;
        }

        /// <summary>
        /// Tick child and return SUCCESS
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Ignore(string name)
        {
            var currentNode = new Succeeder();

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            parentNodes.Push(currentNode);

            return this;
        }

        /*********************/
        /***** COMPOSITE *****/
        /*********************/

        /// <summary>
        /// Add a sequence
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Sequence(string name)
        {
            var currentNode = new Sequence();

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            parentNodes.Push(currentNode);

            return this;
        }

        /// <summary>
        /// Add a selector
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BehaviorTreeBuilder Selector(string name)
        {
            var currentNode = new Selector();

            if (parentNodes.Count > 0)
            {
                (parentNodes.Peek() as Branch).AddChild(currentNode);
            }

            parentNodes.Push(currentNode);

            return this;
        }

        /// <summary>
        /// Close children group
        /// </summary>
        /// <returns></returns>
        public BehaviorTreeBuilder End()
        {
            if (parentNodes.Count > 1)
            {
                parentNodes.Pop();
            }
            return this;
        }

        public static implicit operator BehaviorTree(BehaviorTreeBuilder btb)
        {
            return new BehaviorTree(btb.name, btb.parentNodes.Pop());
        }
    }
}
