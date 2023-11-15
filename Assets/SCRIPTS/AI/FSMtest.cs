using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;

public class FSMtest : BehaviourRunner
{
	[SerializeField] private String Patruyar_action_message;
	[SerializeField] private String Dormir_action_message;
	[SerializeField] private String Avisar_action_message;
	
	
	protected override BehaviourGraph CreateGraph()
	{
		FSM newbehaviourgraph = new FSM();
		
		DebugLogAction Patruyar_action = new DebugLogAction();
		Patruyar_action.message = Patruyar_action_message;
		State Patruyar = newbehaviourgraph.CreateState(Patruyar_action);
		
		DebugLogAction Dormir_action = new DebugLogAction();
		Dormir_action.message = Dormir_action_message;
		State Dormir = newbehaviourgraph.CreateState(Dormir_action);
		
		DebugLogAction Avisar_action = new DebugLogAction();
		Avisar_action.message = Avisar_action_message;
		State Avisar = newbehaviourgraph.CreateState(Avisar_action);
		
		UnityTimePerception EnemigoVisto_perception = new UnityTimePerception();
		EnemigoVisto_perception.TotalTime = 3f;
		StateTransition EnemigoVisto = newbehaviourgraph.CreateTransition(Patruyar, Avisar, EnemigoVisto_perception);
		
		UnityTimePerception HeAvisado_perception = new UnityTimePerception();
		HeAvisado_perception.TotalTime = 3f;
		StateTransition HeAvisado = newbehaviourgraph.CreateTransition(Avisar, Dormir, HeAvisado_perception);
		
		UnityTimePerception Dormido_perception = new UnityTimePerception();
		Dormido_perception.TotalTime = 3f;
		StateTransition Dormido = newbehaviourgraph.CreateTransition(Dormir, Patruyar, Dormido_perception);
		
		return newbehaviourgraph;
	}
}
