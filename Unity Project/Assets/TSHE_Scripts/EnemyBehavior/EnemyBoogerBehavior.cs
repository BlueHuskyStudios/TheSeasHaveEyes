// Class Name: EnemyBoogerBehavior.cs
// Class Purpose: Represents the enemy behaviors that are unique to a Booger. 
// It expects the gameObject this MonoBehavior is attached to to also have an EnemyGenericBehavior attached to function, otherwise it will do very little.
// The main distinct thing about this one is that it can stretch parts of itself towards the player.
using UnityEngine;
using System.Collections;

public class EnemyBoogerBehavior : MonoBehaviour
{
    
    const float DAMAGE = 5f;
    const float attackRange = 5f;
    const float attackRate = 1.0f;
    const float moveSpeed = 1.0f;
    protected EnemyGenericBehavior genericEnemy;
    
    // Use this for initialization
    void Start()
    {
        genericEnemy = GetComponent<EnemyGenericBehavior>();
        if (genericEnemy != null)
        {
            genericEnemy.UpdateAttackRangeAdjustment(attackRange);
        }
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
                print("Trigger Stay by: " + other.ToString());
            }

            PlayerController playerController;

            //If the parent of the object we're colliding with is the player, Check to see if we are looking at eachother before dealing damage.
            playerController = other.transform.parent.GetComponent<PlayerController>();
            if (playerController != null && genericEnemy.AttackCooldownTime <= 0)
            {
                genericEnemy.SmoothLookAt(playerController.transform.position);
                if (genericEnemy.IsFacingPlayer(attackRange))
                {
                    playerController.TakeDamage(DAMAGE * LibRevel.GetScalarFromDistanceThreshold(genericEnemy.playerHitRay.distance, genericEnemy.attackRangeAdjusted));
                    if (GameController.Testing)
                    {
                        print(Vector3.Distance(transform.position, other.transform.position));
                        print("Base Damage: " + DAMAGE);
                        print("Distance from player: " + genericEnemy.playerHitRay.distance);
                        print("Attack Range Adjusted: " + genericEnemy.attackRangeAdjusted);
                        print("Distance / Attack Range Adjusted: " + (genericEnemy.playerHitRay.distance / genericEnemy.attackRangeAdjusted));
                        print(" 1 - (Distance / Attack Range Adjusted): " + (1 - (genericEnemy.playerHitRay.distance / genericEnemy.attackRangeAdjusted)));
                        
                        print(LibRevel.GetScalarFromDistanceThreshold(genericEnemy.playerHitRay.distance, genericEnemy.attackRangeAdjusted));
                        print(DAMAGE * LibRevel.GetScalarFromDistanceThreshold(genericEnemy.playerHitRay.distance, genericEnemy.attackRangeAdjusted));
                        
                    }
                    genericEnemy.AttackCooldownTime = attackRate; //Reset the cooldown wait time.
                }
            }

            //If the object we're colliding with is the player, immediately attempt to deal damage.
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null && genericEnemy.AttackCooldownTime <= 0)
            {
                genericEnemy.SmoothLookAt(playerController.transform.position);
                playerController.TakeDamage(DAMAGE * LibRevel.GetScalarFromDistanceThreshold(transform.position, other.transform.position, attackRange));
                genericEnemy.AttackCooldownTime = attackRate; //Wait a bit before able to attack again.
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
