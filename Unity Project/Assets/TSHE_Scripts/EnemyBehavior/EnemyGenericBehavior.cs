using UnityEngine;
using System.Collections;

public class EnemyGenericBehavior : MonoBehaviour
{

    const float DAMAGE = 1;
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

    void OnTriggerEnter(Collider other)
    {
        print("Trigger Entered by: " + other.ToString());
    }

    void OnTriggerStay(Collider other)
    {
        print("Trigger Stay by: " + other.ToString());
        PlayerController playerController = other.transform.parent.GetComponent<PlayerController>();
        if (playerController != null && attackCooldownTime <= 0)
        {
            playerController.TakeDamage(DAMAGE);
            attackCooldownTime = 1.0f; //Wait one second before able to attack again.
        }
    }

    void OnTriggerExit(Collider other)
    {
        print("Trigger Left by: " + other.ToString());
    }

}
