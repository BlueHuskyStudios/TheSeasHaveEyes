using UnityEngine;
using System.Collections;

public class PickupBubble : MonoBehaviour {

	public GameObject thePlayer;
	public PlayerController thePlayerControllerScript; //There's uglier-to-write but more-reliable ways of accessing this. Maybe consider it later if we have time. - Moore

	//Declare delegates and events.
	public static event System.Action<float> CollidedWithPlayer; // - Moore

	void OnTriggerEnter(Collider collider)
	{
		GameObject objectCollided = collider.gameObject;
		if (thePlayer != null && objectCollided == thePlayer)
		{
			float airBonus =  Mathf.Ceil(Random.Range(0, 10)); //Ceiling should bump the 0.1f up to a 1.0f.
			//thePlayerControllerScript.AddAir(airBonus);
			//thePlayerControllerScript.AddScore(airBonus); //I don't think the doc ever explains how many points air is worth. - Moore

			//Here's an event version of the above two commands. Same functionality, slightly less coupled.
			if (CollidedWithPlayer != null)
			{
				CollidedWithPlayer(airBonus);
			}

			transform.position += new Vector3(Random.Range(-100, 100), 0f, Random.Range(-100, 100));

		}
	}

	void OnDisable()
	{
		CollidedWithPlayer = null;
	}

	//--START OF USER EVENT METHODS.
	
	
	//--END OF USER EVENT METHODS.
}
