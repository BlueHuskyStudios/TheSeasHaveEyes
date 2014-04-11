// Class Name: EnemyGenericBehavior.cs
// Class Purpose: Represents the enemy behaviors that are common between all enemies in The Seas Have Eyes. 
// While not very threatening on its own, it could still be potentially useful for creating special-case enemies that only use 
// a few of the behaviors associated with an enemy, like having an attack cooldown or checking if it's facing the player.

using UnityEngine;
using System.Collections;

public class EnemyGenericBehavior : MonoBehaviour
{
    //Properties that should not change during runtime.
    public float ContactDistance = 2f;
    float attackRange = 5f;
    public float attackRangeAdjusted; //This is public so that we can view it in the inspector. T

    //Properties that will change frequently.
    public float AttackCooldownTime = 0.0f;
    public RaycastHit playerHitRay = new RaycastHit();

    // Use this for initialization
    void Start()
    {
        UpdateAttackRangeAdjustment(attackRange);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (AttackCooldownTime > 0)
        {
            AttackCooldownTime -= Time.fixedDeltaTime;
        }
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        print("Trigger Entered by: " + other.ToString());
    }

    void OnTriggerStay(Collider other)
    {
        print("Trigger Stay by: " + other.ToString());
        PlayerController playerController = other.transform.parent.GetComponent<PlayerController>();
    }

    void OnTriggerExit(Collider other)
    {
        print("Trigger Left by: " + other.ToString());
    }
    */

	#region Utility Methods
    
    //Casts a ray. If the ray hits a player, then return true. Else, return false.
    public bool IsFacingPlayer(float distance)
    {
        bool result = false;
        
        RaycastHit theHit;
        if (Physics.Raycast(transform.position, transform.forward, out theHit, distance))
        {
            PlayerController thePlayer = theHit.collider.gameObject.GetComponent<PlayerController>();
            if (thePlayer != null)
            {
                result = true;
                playerHitRay = theHit;

                if (GameController.Testing) {print("Enemy has spotted the player! Ray Length: " + theHit.distance);}
            }
            else
            {
                playerHitRay = new RaycastHit();
            }
        } 
        
        return result;
    }

    //Convenience method for the above: Just assumes a distance of 1k
    public bool IsFacingPlayer()
    {
        return IsFacingPlayer(Mathf.Infinity);
    }

    public void SmoothLookAt(Vector3 target)
    {
        Quaternion rotation = Quaternion.LookRotation((new Vector3(target.x, target.y, target.z) - transform.position));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * GameController.DAMPING);
    }
   
    public void UpdateAttackRangeAdjustment(float newRange)
    {
        if (renderer != null)
        {
            attackRange = newRange;
            attackRangeAdjusted = attackRange + renderer.bounds.size.z;
        }
    }
	#endregion Utility Methods

}
