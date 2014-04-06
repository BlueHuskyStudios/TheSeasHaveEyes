// Class Name: EnemyStalkBehavior.cs
// Class Purpose: Represents the enemy behaviors that are unique to a Stalk. 
// It expects the gameObject this MonoBehavior is attached to to also have an EnemyGenericBehavior attached to function, otherwise it will do very little.
// The main distinct thing about this one is that it has a much larger player detection range.

using UnityEngine;
using System.Collections;

public class EnemyStalkBehavior : MonoBehaviour {

	
	const float DAMAGE = 7f;
	const float attackRange = 20f;
	const float attackRate = 2.0f;
    const float moveSpeed = 3.0f;


	
	protected EnemyGenericBehavior genericEnemy;
	
	// Use this for initialization
	void Start()
	{
		genericEnemy = GetComponent<EnemyGenericBehavior>();
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if (genericEnemy != null)
		{
			GameObject thePlayer = LibRevel.FindClosestGameObjectWithTag(gameObject, "Player");
            if (LibRevel.IsWithinDistanceThreshold(gameObject, thePlayer, attackRange))
            {
                LibRevel.FlyTowardsGameObject(gameObject, thePlayer, moveSpeed);
            }
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (GameController.Testing)
		{
			print("Trigger Entered by: " + other.ToString());
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (genericEnemy != null)
		{
			
			if (GameController.Testing) 
			{
				//print("Trigger Stay by: " + other.ToString());
			}
            
            PlayerController playerController; 

            //If the parent of the object we're colliding with is the player, Check to see if we are looking at eachother before dealing damage.
            playerController = other.transform.parent.GetComponent<PlayerController>();
            if (playerController != null && genericEnemy.AttackCooldownTime <= 0)
            {
                genericEnemy.SmoothLookAt(playerController.transform.position);
                if (genericEnemy.IsFacingPlayer(attackRange) || LibRevel.IsWithinDistanceThreshold(gameObject, playerController.gameObject, genericEnemy.ContactDistance))
                {
                    playerController.TakeDamage(DAMAGE);
                    genericEnemy.AttackCooldownTime = attackRate; //Wait one second before able to attack again.
                }
            }
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (GameController.Testing)
		{
			print("Trigger Left by: " + other.ToString());
		}
	}
	
	#region Utility Methods

	
	#endregion Utility Methods
}
