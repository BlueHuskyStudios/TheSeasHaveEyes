using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

    //Property declarations.

    //State Machine states.
    public enum PlayingStates
    {
        Null,
        Playing,
        Paused,
        GameOver
    }

    public PlayingStates currentPlayingState;


    // Use this for initialization
    void Start()
    {
        //By checking for null, this only assigns a default if it is not assigned in the inspector. - Moore
        if (currentPlayingState == PlayingStates.Null)
        {
            currentPlayingState = PlayingStates.Playing;
        }
    }
	
    // Update is called once per frame
    void Update()
    {
        //If we're in the 'playing' state, hide and lock the mouse cursor.
        switch (currentPlayingState)
        {
            case PlayingStates.Playing:
                {
                    Screen.lockCursor = true;
                    break;
                }
            default:
                {
                    Screen.lockCursor = false;
                    break;
                }
        }

	
    }
}
