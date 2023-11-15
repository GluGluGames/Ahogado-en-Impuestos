using BehaviourAPI.UnityToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSmartAgent : SmartAgent
{
    [SerializeField, Range(10f, 200f)] float leisureTime;
    [SerializeField, Range(10f, 200f)] float restTime;
    [SerializeField, Range(10f, 200f)] float hygieneTime;
    [SerializeField, Range(10f, 200f)] float bladderTime;
    [SerializeField, Range(10f, 200f)] float thirstTime;
    [SerializeField, Range(10f, 200f)] float hungerTime;

    [SerializeField] float refreshTime = 0.5f;

    private IEnumerator Start()
    {
        while(true) 
        {
            yield return new WaitForSeconds(0.5f);
            AddNeedValue("leisure", -refreshTime / leisureTime);
            AddNeedValue("rest", -refreshTime / restTime);
            AddNeedValue("hygiene", -refreshTime / hygieneTime);
            AddNeedValue("bladder", -refreshTime / bladderTime);
            AddNeedValue("hunger", -refreshTime / hungerTime);
            AddNeedValue("thirst", -refreshTime / thirstTime);
        }
    }
}
