using BehaviourAPI.UnityToolkit.Demos;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    public static SeatManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }


    [SerializeField] List<SeatSmartObject> seats;

    public SeatSmartObject GetRandomSeat()
    {
        var validSeats = seats.FindAll(s => s.Owner == null).ToList();
        int id = Random.Range(0, validSeats.Count);
        return validSeats[id];
    }

    public SeatSmartObject GetRandomSeat(Vector3 pos, float maxDistance)
    {
        var validSeats = seats.FindAll(s => s.Owner == null && Vector3.Distance(s.transform.position, pos) < maxDistance).ToList();
        int id = Random.Range(0, validSeats.Count);
        return validSeats[id];
    }

    public SeatSmartObject GetClosestSeat(Vector3 pos)
    {
        return seats.FindAll(s => s.Owner == null).OrderBy(s => Vector3.Distance(s.transform.position, pos)).FirstOrDefault();
    }
}
