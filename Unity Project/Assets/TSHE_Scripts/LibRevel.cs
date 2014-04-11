//Author Name: Carlis Moore
//Class Name: LibRevel.cs
//Class Purpose: A variety of utility and convenience methods I've found useful in multiple game development projects compiled together so that I don't have to perform copypasta surgery ever week.
using UnityEngine;
using System.Collections;

public static class LibRevel
{

    const float DAMPING = 7f;

    // User calls this method and passes the tag (as a string) they've applied to objects they wish to find. This will linearly search through all of them and pick the closest one with that tag. O(n).
    public static GameObject FindClosestGameObjectWithTagWhileAvoiding(GameObject performer, string tagToFind, GameObject ignoreThisGameObject)
    {
        GameObject result = null;
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tagToFind);
        
        foreach (GameObject current in allObjects)
        {
            if (current != ignoreThisGameObject && current != performer)
            {
                if (result == null)
                {
                    result = current;
                } else
                {
                    //Only change if the newest object we're looking at is the closest.
                    if (Vector3.Distance(performer.transform.position, result.transform.position) > Vector3.Distance(performer.transform.position, current.transform.position))
                    {
                        result = current;
                        
                    }
                    
                }
                
            }
        }
        return result;
    }

    public static GameObject FindClosestGameObjectWithTag(GameObject performer, string tagToFind)
    {
        return FindClosestGameObjectWithTagWhileAvoiding(performer, tagToFind, null);
    }

    public static GameObject FindClosestGameObjectWithTag(string tagToFind)
    {
        return FindClosestGameObjectWithTag(null, tagToFind);
    }

    //This takes three pairs of min and max values and picks a random Vector3 of floats that are constrainted to those passed values.
    public static Vector3 RandomVector3InRange(float xMin, float xMax, float yMin, float yMax, float zMin, float zMax)
    {
        float tempX = Random.Range(xMin, xMax);
        float tempY = Random.Range(yMin, yMax);
        float tempZ = Random.Range(zMin, zMax);
        
        return new Vector3(tempX, tempY, tempZ);
    }

    public static void FlyTowardsGameObject(GameObject performer, GameObject destination, float movementSpeed)
    {
        //Precondition: You should only be calling this once per caller per FixedUpdate. - Moore
        if (destination != null)
        {
            //Make sure to check that the target is within a desirable distance. So there should be a maximum distance variable. - Moore

            /*
            //Smooth turning position tracking... Maybe consider a version of this follow script that doesn't enforce headturning.
            Quaternion rotation = Quaternion.LookRotation((new Vector3(destination.transform.position.x, destination.transform.position.y, destination.transform.position.z) - transform.position));
            performer.transform.rotation = Quaternion.Slerp(performer.transform.rotation, rotation, Time.deltaTime * DAMPING);
            */
            
            if (IsNotWithinDistanceThreshold(performer, destination, movementSpeed)) //WARNING: This movement speed is an assumption instead of a given threshold. Might be bad to have these two interconnected like this.
            { 
                if (performer.rigidbody == null)
                {
                    performer.transform.position = (Vector3.MoveTowards(performer.transform.position, destination.transform.position, 0.1f));
                } else
                {               
                    performer.rigidbody.freezeRotation = true;
                    performer.rigidbody.velocity = Vector3.zero;
                    performer.rigidbody.AddForce(Vector3.MoveTowards(performer.transform.position, destination.transform.position, movementSpeed * Time.fixedDeltaTime));        
                }       
            }
        }
    }

    //Given two gameObjects and a float distance threshold, will check to see if the distance between the objects is within the threshold.
    public static bool IsWithinDistanceThreshold(GameObject performer, GameObject destination, float threshold)
    {
        bool result = false;
        
        if (Vector3.Distance(performer.transform.position, destination.transform.position) > threshold)
        { //If it is outside of the threshold...
            result = false; //Return false. - Moore
        } else
        { // Otherwise, return true. - Moore
            return true;
        }
        
        return result;
    }
    
    //Convenience Negation Wrapper Method for the above:
    public static bool IsNotWithinDistanceThreshold(GameObject performer, GameObject destination, float threshold)
    {
        return !IsWithinDistanceThreshold(performer, destination, threshold);
    }
}
