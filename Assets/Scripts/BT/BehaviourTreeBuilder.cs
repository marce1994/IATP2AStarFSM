using System;
using System.Collections.Generic;

namespace BehaviorTree
{
    // https://github.com/ashleydavis/Fluent-Behaviour-Tree/blob/master/src/BehaviourTreeBuilder.cs
    public class BehaviourTreeBuilder
    {
        private BehaviorComponent curNode = null;

        private Stack<BehaviorComponent> parentNodeStack = new Stack<BehaviorComponent>();

        /// <summary>
        /// Create an action node.
        /// </summary>
        public BehaviourTreeBuilder Do(string name, BehaviourActionDelegate fn)
        {
            if (parentNodeStack.Count <= 0)
            {
                throw new ApplicationException("Can't create an unnested ActionNode, it must be a leaf node.");
            }

            var actionNode = new BehaviorAction(fn);
            parentNodeStack.Peek().AddChild(actionNode);
            return this;
        }

        /// <summary>
        /// Like an action node... but the function can return true/false and is mapped to success/failure.
        /// </summary>
        public BehaviourTreeBuilder Condition(string name, ConditionalDelegate fn)
        {
            if (parentNodeStack.Count <= 0)
            {
                throw new ApplicationException("Can't create an unnested ActionNode, it must be a leaf node.");
            }

            var conditionalNode = new Conditional(fn);
            parentNodeStack.Peek().AddChild(conditionalNode);
            return this;
        }

        /// <summary>
        /// Create an inverter node that inverts the success/failure of its children.
        /// </summary>
        public BehaviourTreeBuilder Inverter(string name)
        {
            var inverterNode = new Inverter();

            if (parentNodeStack.Count > 0)
            {
                parentNodeStack.Peek().AddChild(inverterNode);
            }

            parentNodeStack.Push(inverterNode);
            return this;
        }

        /// <summary>
        /// Create a sequence node.
        /// </summary>
        public BehaviourTreeBuilder Sequence(string name)
        {
            var sequenceNode = new Sequence();

            if (parentNodeStack.Count > 0)
            {
                parentNodeStack.Peek().AddChild(sequenceNode);
            }

            parentNodeStack.Push(sequenceNode);
            return this;
        }

        /// <summary>
        /// Create a stateful sequence node.
        /// </summary>
        public BehaviourTreeBuilder StateFulSequence(string name)
        {
            var sequenceNode = new StatefulSequence();

            if (parentNodeStack.Count > 0)
            {
                parentNodeStack.Peek().AddChild(sequenceNode);
            }

            parentNodeStack.Push(sequenceNode);
            return this;
        }

        /// <summary>
        /// Create a selector node.
        /// </summary>
        public BehaviourTreeBuilder Selector()
        {
            var selectorNode = new Selector();

            if (parentNodeStack.Count > 0)
            {
                parentNodeStack.Peek().AddChild(selectorNode);
            }

            parentNodeStack.Push(selectorNode);
            return this;
        }

        /// <summary>
        /// Splice a sub tree into the parent tree.
        /// </summary>
        public BehaviourTreeBuilder Splice(BehaviorComponent subTree)
        {
            if (subTree == null)
            {
                throw new ArgumentNullException("subTree");
            }

            if (parentNodeStack.Count <= 0)
            {
                throw new ApplicationException("Can't splice an unnested sub-tree, there must be a parent-tree.");
            }

            parentNodeStack.Peek().AddChild(subTree);
            return this;
        }

        /// <summary>
        /// Build the actual tree.
        /// </summary>
        public BehaviorComponent Build()
        {
            if (curNode == null)
            {
                throw new ApplicationException("Can't create a behaviour tree with zero nodes");
            }
            return curNode;
        }

        /// <summary>
        /// Ends a sequence of children.
        /// </summary>
        public BehaviourTreeBuilder End()
        {
            curNode = parentNodeStack.Pop();
            return this;
        }
    }
}