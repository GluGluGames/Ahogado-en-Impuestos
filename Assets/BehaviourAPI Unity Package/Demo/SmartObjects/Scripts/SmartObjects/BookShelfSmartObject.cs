using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using Core.Actions;
    using SmartObjects;
    using UnityToolkit;

    public class BookShelfSmartObject : DirectSmartObject
    {
        public override bool ValidateAgent(SmartAgent agent)
        {
            // Comprobar si hay asientos libres
            return true;
        }

        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            var seatAction = new SeatRequestAction(agent);
            return seatAction;
        }
    }
}