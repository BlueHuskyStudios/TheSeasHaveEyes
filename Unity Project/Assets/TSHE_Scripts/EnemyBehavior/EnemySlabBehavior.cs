using UnityEngine;
using System.Collections;

public class EnemySlabBehavior : MonoBehaviour {

    const float DAMAGE = 1;

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
                playerController.TakeDamage(DAMAGE);
                genericEnemy.attackCooldownTime = 1.0f; //Wait one second before able to attack again.
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
