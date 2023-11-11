using BehaviourAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.BehaviourTrees
{
    /// <summary>
    /// Composite node that selects one of its branch based on a probability.
    /// </summary>
    public class ProbabilityBranchNode : BranchNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        /// <summary>
        /// The last random value generated to select branch(only for debug).
        /// </summary>
        public double RandomValue { get; private set; }

        /// <summary>
        /// The list of probabilities assigned to each branch. To set the probability using the
        /// child node directly, use <see cref="SetProbability"/> method.
        /// </summary>
        public List<float> probabilities = new List<float>();

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        static Random Random = new Random();

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the probability of the child node. 
        /// </summary>
        /// <param name="node">The specified node.</param>
        /// <param name="probability">The new probablity of the node.</param>
        public void SetProbability(BTNode node, float probability)
        {
            int index = Children.IndexOf(node);

            if (index == -1) return;

            for (int i = probabilities.Count; i <= index; i++) probabilities.Add(0);
            probabilities[index] = probability;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Gets a child based on its probabilities and a random generated value.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override int SelectBranchIndex()
        {
            var totalProbability = probabilities.Where(p => p > 0f).Sum();
            var probability = Random.NextDouble() * totalProbability;
            RandomValue = probability;
            var currentProbSum = 0f;
            int selectedChild = 0;

            for (int i = 0; i < m_children.Count; i++)
            {
                BTNode node = m_children[i];
                if (node == null) continue;

                if(probabilities.Count > i && probabilities[i] > 0f)
                {
                    currentProbSum += probabilities[i];
                    if (currentProbSum > probability)
                    {
                        selectedChild = i;
                        break;
                    }
                }
            }
            return selectedChild;
        }

        /// <summary>
        /// Get the probability assigned to a child node.
        /// If the node doesn't have a probability assigned, return 0.
        /// </summary>
        /// <param name="node">The specified node.</param>
        /// <returns>The probability assigned to the node.</returns>
        public float GetProbability(BTNode node)
        {
            int index = Children.IndexOf(node);
            if (probabilities.Count > index)
                return probabilities[index];
            else
                return 0f;
        }

        #endregion
    }
}
