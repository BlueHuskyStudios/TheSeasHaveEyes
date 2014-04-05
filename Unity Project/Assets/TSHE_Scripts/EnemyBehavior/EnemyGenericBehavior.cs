using UnityEngine;
using System.Collections;

public class EnemyGenericBehavior : MonoBehaviour
{

    public float attackCooldownTime = 0.0f;

    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (attackCooldownTime > 0)
        {
            attackCooldownTime -= Time.fixedDeltaTime;
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
                if (GameController.Testing) {print("Enemy has spotted the player!");}
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
        Quaternion rotation = Quaternion.LookRotation((new Vector3(target.x, transform.position.y, target.z) - transform.position));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * GameController.DAMPING);
    }

	#endregion Utility Methods

}
