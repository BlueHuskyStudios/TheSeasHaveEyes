using UnityEngine;
using System.Collections;

public class EnemySlabBehavior : MonoBehaviour {

    const float DAMAGE = 1f;
    const float attackRange = 10f;

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
            //Do stuff unique to Slabs.
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
            PlayerController playerController = other.transform.parent.GetComponent<PlayerController>();
            if (playerController != null && genericEnemy.attackCooldownTime <= 0)
            {
                genericEnemy.SmoothLookAt(playerController.transform.position);
                if (genericEnemy.IsFacingPlayer(attackRange))
                {
                    playerController.TakeDamage(DAMAGE);
                    genericEnemy.attackCooldownTime = 1.0f; //Wait one second before able to attack again.
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

    //I should move this into its own utility script class. - Moore
    protected GameObject FindClosestGameObjectWithTag(string tagToFind)
    {
        GameObject result = null;
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tagToFind);
        
        foreach (GameObject current in allObjects)
        {
            if (current != this.gameObject)
            {
                if (result == null)
                {
                    result = current;
                } else
                {
                    if (Vector3.Distance(transform.position, result.transform.position) > Vector3.Distance(transform.position, current.transform.position))
                    {
                        result = current;
                    }
                }
            }
        }
        return result;
    }
	
	#endregion Utility Methods
}
