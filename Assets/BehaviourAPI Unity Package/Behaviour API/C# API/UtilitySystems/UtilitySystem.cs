using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UtilitySystems
{
    using Core;
    using Core.Actions;

    /// <summary>
    /// Behaviour graph that choose between diferent <see cref="UtilitySelectableNode"/> items and executes.
    /// </summary>
    public class UtilitySystem : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(UtilityNode);

        public override bool CanRepeatConnection => false;
        public override bool CanCreateLoops => false;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// The utility multiplier of the selected element and prevents it from jittering.
        /// </summary>
        public float Inertia = 1.3f;

        #endregion

        List<UtilitySelectableNode> _utilityCandidates;
        List<UtilityNode> _utilityNodes;

        UtilitySelectableNode _currentBestElement;

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Creates a new <see cref="UtilitySystem"/>
        /// </summary>       
        public UtilitySystem()
        {
            Inertia = 1.3f;
            _utilityNodes = new List<UtilityNode>();
            _utilityCandidates = new List<UtilitySelectableNode>();
        }

        /// <summary>
        /// Creates a new <see cref="UtilitySystem"/>
        /// </summary>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        public UtilitySystem(float inertia = 1.3f)
        {
            Inertia = inertia;
            _utilityNodes = new List<UtilityNode>();
            _utilityCandidates = new List<UtilitySelectableNode>();
        }

        /// <summary>
        /// Use this method to instantiate custom types of leaf factors.
        /// </summary>
        /// <typeparam name="T">The type of the leaf factor.</typeparam>
        /// <returns>The factor of type <typeparamref name="T"/> created.</returns>
        public T CreateLeaf<T>() where T : LeafFactor, new()
        {
            T leafFactor = CreateNode<T>();
            return leafFactor;
        }

        /// <summary>
        /// Use this method to instantiate custom types of leaf factors with name.
        /// </summary>
        /// <typeparam name="T">The type of the leaf factor.</typeparam>
        /// <param name="name">The name of the factor</param>
        /// <returns>The factor of type <typeparamref name="T"/> created.</returns>
        public T CreateLeaf<T>(string name) where T : LeafFactor, new()
        {
            T leafFactor = CreateNode<T>(name);
            return leafFactor;
        }

        /// <summary>
        /// Create a new <see cref="VariableFactor"/> named <paramref name="name"/> in this <see cref="UtilitySystem"/> that computes its utility value with the result
        /// of the delegate function specified in <paramref name="func"/>, normalized between 0 and 1 using <paramref name="min"/> and <paramref name="max"/> values. 
        /// </summary>
        /// <param name="name">The name of the variable factor.</param>
        /// <param name="func">The function delegate that executes this factor.</param>
        /// <param name="min">The minimum expected value of the result of <paramref name="func"/></param>
        /// <param name="max">The maximum expected value of the result of <paramref name="func"/></param>
        /// <returns>The <see cref="VariableFactor"/> created.</returns>
        public VariableFactor CreateVariable(string name, Func<float> func, float min, float max)
        {
            VariableFactor variableFactor = CreateLeaf<VariableFactor>(name);
            variableFactor.Variable = func;
            variableFactor.min = min;
            variableFactor.max = max;
            return variableFactor;
        }

        /// <summary>
        /// Create a new <see cref="VariableFactor"/> in this <see cref="UtilitySystem"/> that computes its utility value with the result
        /// of the delegate function specified in <paramref name="func"/>, normalized between 0 and 1 using <paramref name="min"/> and <paramref name="max"/> values. 
        /// </summary>
        /// <param name="func">The function delegate that executes this factor.</param>
        /// <param name="min">The minimum expected value of the result of <paramref name="func"/></param>
        /// <param name="max">The maximum expected value of the result of <paramref name="func"/></param>
        /// <returns>The <see cref="VariableFactor"/> created.</returns>
        public VariableFactor CreateVariable(Func<float> func, float min, float max)
        {
            VariableFactor variableFactor = CreateLeaf<VariableFactor>();
            variableFactor.Variable = func;
            variableFactor.min = min;
            variableFactor.max = max;
            return variableFactor;
        }

        /// <summary>
        /// Create a new <see cref="ConstantFactor"/> in this <see cref="ConstantFactor"/> with a constant utility value.       /// </summary>
        /// <param name="name">The name of the variable factor.</param>  
        /// <param name="value">The utility value</param> 
        /// <returns>The <see cref="ConstantFactor"/> created.</returns>
        public ConstantFactor CreateConstant(string name, float value)
        {
            ConstantFactor constantFactor = CreateLeaf<ConstantFactor>(name);
            constantFactor.value = value;

            return constantFactor;
        }

        /// <summary>
        /// Create a new <see cref="ConstantFactor"/> in this <see cref="ConstantFactor"/> with a constant utility value.       /// </summary>
        /// <param name="value">The utility value</param> 
        /// <returns>The <see cref="ConstantFactor"/> created.</returns>
        public ConstantFactor CreateConstant(float value)
        {
            ConstantFactor constantFactor = CreateNode<ConstantFactor>();
            constantFactor.value = value;
            return constantFactor;
        }

        /// <summary>
        /// Create a new function factor of type <typeparamref name="T"/> named <paramref name="name"/> that computes its utility value modifying the utility of <paramref name="child"/> factor.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="name">The name of the factor.</param>
        /// <param name="child">The child factor.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateCurve<T>(string name, Factor child) where T : CurveFactor, new()
        {
            T curveFactor = CreateNode<T>(name);
            Connect(curveFactor, child);
            curveFactor.SetChild(child);
            return curveFactor;
        }

        /// <summary>
        /// Create a new function factor of type <typeparamref name="T"/> that computes its utility value modifying the utility of <paramref name="child"/> factor.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="child">The child factor.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateCurve<T>(Factor child) where T : CurveFactor, new()
        {
            T curveFactor = CreateNode<T>();
            Connect(curveFactor, child);
            curveFactor.SetChild(child);
            return curveFactor;
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> named <paramref name="name"/> that combines the utility of <paramref name="factors"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="name">The name of the factor.</param>
        /// <param name="factors">The list of child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusion<T>(string name, List<Factor> factors) where T : FusionFactor, new()
        {
            T fusionFactor = CreateNode<T>(name);
            factors.ForEach(factor =>
            {
                Connect(fusionFactor, factor);
                fusionFactor.AddFactor(factor);
            });
            return fusionFactor;
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> that combines the utility of <paramref name="factors"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="factors">The list of child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusion<T>(List<Factor> factors) where T : FusionFactor, new()
        {
            T fusionFactor = CreateNode<T>();
            factors.ForEach(factor =>
            {
                Connect(fusionFactor, factor);
                fusionFactor.AddFactor(factor);
            });
            return fusionFactor;
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> named <paramref name="name"/> that combines the utility of <paramref name="children"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="name">The name of the factor.</param>
        /// <param name="children">The child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusion<T>(string name, params Factor[] children) where T : FusionFactor, new()
        {
            return CreateFusion<T>(name, children.ToList());
        }

        /// <summary>
        /// Create a new fusion factor of type <typeparamref name="T"/> that combines the utility of <paramref name="children"/>.
        /// </summary>
        /// <typeparam name="T">The type of the factor.</typeparam>
        /// <param name="children">The child factors.</param>
        /// <returns>The <typeparamref name="T"/> created.</returns>
        public T CreateFusion<T>(params Factor[] children) where T : FusionFactor, new()
        {
            return CreateFusion<T>(children.ToList());
        }



        /// <summary>
        /// Create a new <see cref="UtilityAction"/> named <paramref name="name"/> that computes its utility using <paramref name="factor"/> and executes the action specified in <paramref name="action"/>.
        /// To prevent the action from being added to the <see cref="UtilitySystem"/> candidate list, set <paramref name="root"/> to false (default is true).
        /// To make the <see cref="UtilitySystem"/> execution ends when the action ends, set <paramref name="finishOnComplete"/> to true (default is false).
        /// </summary>
        /// <param name="name">The name of the utility action.</param>
        /// <param name="factor">The child factor of the action.</param>
        /// <param name="action">The action executed.</param>
        /// <param name="finishOnComplete">true of the execution of the utility system must finish when the action finish.</param>
        /// <param name="group">The bucket that contains the element. If its null the element will be added to candidates in the <see cref="UtilitySystem"/></param>
        /// <returns>The created <see cref="UtilityAction"/></returns>
        public UtilityAction CreateAction(string name, Factor factor, Action action = null, bool finishOnComplete = false, UtilityBucket group = null)
        {
            UtilityAction utilityAction = CreateNode<UtilityAction>(name);
            utilityAction.Action = action;
            utilityAction.FinishSystemOnComplete = finishOnComplete;

            Connect(utilityAction, factor);
            utilityAction.SetFactor(factor);

            AddToGroup(utilityAction, group);

            return utilityAction;
        }

        /// <summary>
        /// Create a new <see cref="UtilityAction"/> that computes its utility using <paramref name="factor"/> and executes the action specified in <paramref name="action"/>.
        /// To prevent the action from being added to the <see cref="UtilitySystem"/> candidate list, set <paramref name="root"/> to false (default is true).
        /// To make the <see cref="UtilitySystem"/> execution ends when the action ends, set <paramref name="finishOnComplete"/> to true (default is false).
        /// </summary>
        /// <param name="factor">The child factor of the action.</param>
        /// <param name="action">The action executed.</param>
        /// <param name="finishOnComplete">true of the execution of the utility system must finish when the action finish.</param>
        /// <param name="group">The bucket that contains the element. If its null the element will be added to candidates in the <see cref="UtilitySystem"/></param>
        /// <returns>The created <see cref="UtilityAction"/></returns>
        public UtilityAction CreateAction(Factor factor, Action action = null, bool finishOnComplete = false, UtilityBucket group = null)
        {
            UtilityAction utilityAction = CreateNode<UtilityAction>();
            utilityAction.Action = action;
            utilityAction.FinishSystemOnComplete = finishOnComplete;

            Connect(utilityAction, factor);
            utilityAction.SetFactor(factor);

            AddToGroup(utilityAction, group);

            return utilityAction;
        }



        /// <summary>
        /// Create a new <see cref="UtilityExitNode"/> named <paramref name="name"/> that computes its utility using <paramref name="factor"/> and exit the current graph execution with the value of <paramref name="exitStatus"/>.
        /// To prevent the action from being added to the <see cref="UtilitySystem"/> candidate list, set <paramref name="root"/> to false (default is true).
        /// </summary>
        /// <param name="name">The name of the utility action.</param>
        /// <param name="factor">The child factor of the action.</param>
        /// <param name="group">The bucket that contains the element. If its null the element will be added to candidates in the <see cref="UtilitySystem"/></param>
        /// <returns>The created <see cref="UtilityExitNode"/></returns>
        public UtilityExitNode CreateExitNode(string name, Factor factor, Status exitStatus, UtilityBucket group = null)
        {
            UtilityExitNode exitNode = CreateNode<UtilityExitNode>(name);
            exitNode.ExitStatus = exitStatus;

            Connect(exitNode, factor);
            exitNode.SetFactor(factor);

            AddToGroup(exitNode, group);

            return exitNode;
        }

        /// <summary>
        /// Create a new <see cref="UtilityExitNode"/> named <paramref name="name"/> that computes its utility using <paramref name="factor"/> and exit the current graph execution with the value of <paramref name="exitStatus"/>.
        /// To prevent the action from being added to the <see cref="UtilitySystem"/> candidate list, set <paramref name="root"/> to false (default is true).
        /// </summary>
        /// <param name="factor">The child factor of the action.</param>
        /// <param name="group">The bucket that contains the element. If its null the element will be added to candidates in the <see cref="UtilitySystem"/></param>
        /// <returns>The created <see cref="UtilityExitNode"/></returns>
        public UtilityExitNode CreateExitNode(Factor factor, Status exitStatus, UtilityBucket group = null)
        {
            UtilityExitNode exitNode = CreateNode<UtilityExitNode>();
            exitNode.ExitStatus = exitStatus;

            Connect(exitNode, factor);
            exitNode.SetFactor(factor);

            AddToGroup(exitNode, group);

            return exitNode;
        }



        /// <summary>
        /// Create a new <see cref="UtilityBucket"/> in this <see cref="UtilitySystem"/> that groups the elements specified in <paramref name="elements"/>.
        /// </summary> 
        /// <param name="name">The name of the utility action.</param>
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="bucketThreshold">The minimum utility this bucket must have to get priority.</param>
        /// <param name="group">The bucket that contains the element. If its null the element will be added to candidates in the <see cref="UtilitySystem"/></param>
        /// <returns>The <see cref="UtilityBucket"/> created.</returns>
        public UtilityBucket CreateBucket(string name, float inertia = 1.3f, float bucketThreshold = 0f, UtilityBucket group = null)
        {
            UtilityBucket bucket = CreateNode<UtilityBucket>(name);
            bucket.Inertia = inertia;
            bucket.BucketThreshold = bucketThreshold;

            AddToGroup(bucket, group);

            return bucket;
        }

        /// <summary>
        /// Create a new <see cref="UtilityBucket"/> in this <see cref="UtilitySystem"/> that groups the elements specified in <paramref name="elements"/>.
        /// </summary> 
        /// <param name="inertia">The utility multiplier applied to the last selected element when the best element is calculated.</param>
        /// <param name="bucketThreshold">The minimum utility this bucket must have to get priority.</param>
        /// <param name="group">The bucket that contains the element. If its null the element will be added to candidates in the <see cref="UtilitySystem"/></param>
        /// <returns>The <see cref="UtilityBucket"/> created.</returns>
        public UtilityBucket CreateBucket(float inertia = 1.3f, float bucketThreshold = 0f, UtilityBucket group = null)
        {
            UtilityBucket bucket = CreateNode<UtilityBucket>();
            bucket.Inertia = inertia;
            bucket.BucketThreshold = bucketThreshold;

            AddToGroup(bucket, group);

            return bucket;
        }

        /// <summary>
        /// <inheritdoc/>
        /// If the node is an <see cref="UtilityNode"/>, add to the internal list.
        /// </summary>
        /// <param name="node"></param>
        protected override void AddNode(Node node)
        {
            base.AddNode(node);

            if (node is UtilityNode utilityNode)
                _utilityNodes.Add(utilityNode);
        }

        public override object Clone()
        {
            var us = (UtilitySystem)base.Clone();
            us._utilityCandidates = new List<UtilitySelectableNode>();
            return us;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Create the main <see cref="UtilitySelectableNode"/> candidate list.
        /// </summary>
        protected override void Build()
        {
            foreach (Node node in Nodes)
            {
                if (node is UtilitySelectableNode selectableNode && selectableNode.ParentCount == 0)
                    _utilityCandidates.Add(selectableNode);
            }
        }

        private void AddToGroup(UtilitySelectableNode element, UtilityBucket group)
        {
            if (group != null)
            {
                Connect(group, element);
                group.AddElement(element);
            }
            else
            {
                _utilityCandidates.Add(element);
            }
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Throws and error if there are no <see cref="UtilitySelectableNode"/> in the graph.
        /// </summary>
        /// <exception cref="EmptyGraphException">If the candidate list is empty.</exception>
        protected override void OnStarted()
        {
            if (_utilityCandidates.Count == 0)
                throw new EmptyGraphException(this, "The list of utility candidates is empty.");
        }

        /// <summary>
        /// <inheritdoc/>
        /// Recalculates the utilities of the nodes and selects the best candidate to run it.
        /// If the new candidate chosen is different from the one of the previous iteration, it stops its execution and starts the new one.
        /// </summary>
        protected override void OnUpdated()
        {
            foreach (UtilityNode node in _utilityNodes) node.MarkUtilityAsDirty();

            var newBestAction = ComputeCurrentBestAction();
            // If the best action changes:
            if (newBestAction != _currentBestElement)
            {
                _currentBestElement?.OnStopped();
                _currentBestElement = newBestAction;
                _currentBestElement?.OnStarted();
            }
            _currentBestElement?.OnUpdated();
        }



        private UtilitySelectableNode ComputeCurrentBestAction()
        {
            float currentHigherUtility = -1f; // If value starts in 0, elems with Utility == 0 cant be executed

            UtilitySelectableNode newBestElement = null;

            int i = 0;
            var currentElementIsLocked = false; // True if current action is a locked bucket.

            while (i < _utilityCandidates.Count && !currentElementIsLocked)
            {
                // Update utility
                var currentCandidate = _utilityCandidates[i];

                if (currentCandidate == null) continue;

                currentCandidate.UpdateUtility();

                var utility = currentCandidate.Utility;
                if (currentCandidate == _currentBestElement) utility *= Inertia;

                // If it's higher than the current max utility, update the selection.
                if (utility > currentHigherUtility)
                {
                    newBestElement = currentCandidate;
                    currentHigherUtility = utility;
                }

                // If the current candidate is a locked bucket:
                if (currentCandidate.ExecutionPriority) currentElementIsLocked = true;

                i++;
            }

            return newBestElement;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stops the current best element execution.
        /// </summary>
        protected override void OnStopped()
        {
            _currentBestElement?.OnStopped();
            _currentBestElement = null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the current best element execution.
        /// </summary>
        protected override void OnPaused()
        {
            _currentBestElement?.OnPaused();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses the current best element execution.
        /// </summary>
        protected override void OnUnpaused()
        {
            _currentBestElement?.OnUnpaused();
        }

        #endregion
    }
}