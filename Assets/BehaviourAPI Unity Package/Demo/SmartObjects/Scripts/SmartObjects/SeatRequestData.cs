using BehaviourAPI.SmartObjects;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class SeatRequestData : RequestData
    {
        public float useTime;

        public SeatRequestData(float useTime)
        {
            this.useTime = useTime;
        }
    }
}