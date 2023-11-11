using BehaviourAPI.StateMachines;

namespace BehaviourAPI.UnityToolkit.Demos
{
	public interface IRadar
	{
		public State GetBrokenState();
		public State GetWorkingState();
	}

}