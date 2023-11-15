namespace BehaviourAPI.Core
{
    /// <summary>
    /// Implement this interface in a node to make it activable externally.
    /// </summary>
    public interface IPushActivable
    {
        void Fire(Status status);
    }
}
