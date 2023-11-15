﻿namespace BehaviourAPI.UnityToolkit.Demos
{
	using UnityEngine;

	public class DestroyEffect : MonoBehaviour
	{

		void Update()
		{

			if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C))
				Destroy(transform.gameObject);

		}
	}
}
