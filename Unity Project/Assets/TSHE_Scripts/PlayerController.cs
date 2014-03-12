using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{


    //Property Instantiation.

    //--Constants and other variables that shouldn't be changed during runtime in the final product.
    public float movespeed; //This is set in the inspector to determine the forward-movement speed of the character. - Moore



    //Change these to change the colors of the drawn status bars.
    public Texture2D blackPixelTex;
    public Texture2D airPixelTex;
    public Texture2D healthPixelTex;
    public Texture2D staminaPixelTex;
    public Texture2D weaponPixelTex;


    //--Variables that are expected to change a lot during runtime.
    public float healthMax = 100f;
    public float health;

    public float staminaMax = 100f;
    public float staminaGainRate = 0.01f;
    public float staminaLossRate = 1f;
    public float staminaBasedAirLossrate = 0.1f;
    public float stamina;

    public float airMax = 100f;
    public float airLossRate = 1f; //This 1 means 1 per second. to make it calc by seconds, we'll multiply by 'Time.fixedDeltaTime' - Moore
    public float air;

    public float score = 0f;

    public float movementScalar = 20f;
    public float movementBoostScalar = 0f; //This scalar and the above scalar are additive. So if both were 20, the total speedboost would be *40, not *400.

    public float gamepadLeftHorizontalOffset;
    public float gamepadLeftVerticalOffset;
    public float gamepadRightHorizontalOffset;
    public float gamepadRightVerticalOffset;
    public float gamepadTriggerOffset;

    //--START OF EVENT METHODS

    // Use this for initialization
    void Start()
    {
        health = healthMax;
        stamina = staminaMax;
        air = airMax;
	
    }
	
    // FixedUpdate is called once per frame
    void FixedUpdate()
    {

        HandlePlayerInput();

        air -= airLossRate * Time.fixedDeltaTime;
        if (air < 0)
        {
            //If the player is out of air, then they rapidly lose health? Or instant game-over? - Moore
            health -= airLossRate * Time.fixedDeltaTime * 2; //This is assuming drowning drains health gradually. Replace this if it's insta-lose. - Moore
            if (health < 0) //Ditto. - Moore
            {
                health = 0;
                //TODO: Game Over Here!
            }
            air = 0;
        }

        if (gamepadTriggerOffset != 0.0f)
        {
            stamina -= staminaLossRate * Time.fixedDeltaTime;
            air -= staminaBasedAirLossrate * Time.fixedDeltaTime;

            if (stamina < 0)
            {
                //If the player is out of air, then they rapidly lose health? Or instant game-over? - Moore
                health -= airLossRate * Time.fixedDeltaTime * 2; //This is assuming drowning drains health gradually. Replace this if it's insta-lose. - Moore
                if (health < 0) //Ditto. - Moore
                {
                    health = 0;
                    //TODO: Game Over Here!
                }
                air = 0;
            }
        }

        if (stamina < staminaMax)
        {
            stamina += staminaGainRate * Time.fixedDeltaTime;
            if (stamina > staminaMax)
            {
                stamina = staminaMax;
            }
        }


        //rigidbody.velocity = Vector3.zero; - How the frig do we make the thing not fly off when it touches the ground? And not have the terrain fly off either? Either way, we need to stop the passthrough. - Moore
    }

    void OnGUI()
    {
        DrawHUD();
    }

    /*
	void OnTriggerEnter(Collider other)
	{
		score += 100;
		air += 10;
		Destroy(other.gameObject); //Again, probably not the best place to put this, but it should work for now. - Moore
	}
	*/

    //--END OF EVENT METHODS.

    void DrawHUD()
    {
        //This method might be better suited for a general gamecontroller script instead of the Player, but for now it's easier to access the variables from here.

        //And now for a little bit of reuse from Diurnal Code. Gonna need tweaking to play well with TSHE. - Moore
        //This draws a meter. It's in a dedicated block block to limit the scope of the variables to just that block. If you really need access to those variables somewhere else, or if you want to instantiate them elsewhere, then remove the block. - Moore
        {
            DrawMeter(new Vector2(1, Screen.height - 20), new Vector2(((Screen.width / 4) - 1), Screen.height / 50), health, healthMax, healthPixelTex); //Draw the health Meter on the bottom-left of the screen.
            DrawMeter(new Vector2((Screen.width / 4) + 1, Screen.height - 20), new Vector2((Screen.width / 4) - 1, Screen.height / 50), air, airMax, airPixelTex); //Draw the health Meter on the bottom-left of the screen.
            DrawMeter(new Vector2((Screen.width * 2 / 4) + 1, Screen.height - 20), new Vector2((Screen.width / 4) - 1, Screen.height / 50), stamina, staminaMax, staminaPixelTex); //Draw the health Meter on the bottom-left of the screen.
        }
    }

    void DrawMeter(Vector2 leftTop, Vector2 widthHeight, float varCurrent, float varMax, Texture2D fillTexture)
    {
        //Variables to handle positioning and scaling for the back-bar and the front-bar.
        float barLeft = leftTop.x;
        float barTop = leftTop.y;

        float barWidth = widthHeight.x;
        float barHeight = widthHeight.y;
		
        //GUI.Label(new Rect(barLeft, barTop, barWidth, barHeight), "THE END IS NEVER THE END IS NEVER THE END IS NEVER THE END IS NEVER THE END IS NEVER THE END IS NEVER THE END IS NEVER THE END IS NEVER ");
        //GUI.Label(new Rect(barLeft, barTop, barWidth, barHeight), "", backBarGUIStyle);
		
        GUI.color = Color.black;
        GUI.DrawTexture(new Rect(barLeft, barTop, barWidth, barHeight), blackPixelTex);
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(barLeft, barTop, barWidth * ((float)varCurrent / varMax), barHeight), fillTexture);
        GUI.color = Color.white;
    }


    void HandlePlayerInput()
    {

        /* Leggiero - 
		* Keyboard WASD – Forward, Left strafe, Backward, Right strafe (respectively)
		* Keyboard Space or Shift – Boost (takes up stamina)
		* Keyboard Ctrl – Brake
		* Mouse movement – Pitch, yaw
		* Mouse left click – Forward - Leggiero.
		* Mouse right click - Attack? Or is attacking automatic? - Moore
		*/

        //Movement Section

        gamepadLeftHorizontalOffset = Input.GetAxis("Horizontal");
        gamepadLeftVerticalOffset = Input.GetAxis("Vertical");
		
        gamepadRightHorizontalOffset = Input.GetAxis("RightHorizontal");
        gamepadRightVerticalOffset = Input.GetAxis("RightVertical");
		
        gamepadTriggerOffset = Input.GetAxis("Trigger");
        if (gamepadTriggerOffset >= 0)
        {
            movementBoostScalar = (20.0f * gamepadTriggerOffset); //Makes the modifier range from 0.0 to 20.0 assuming the trigger is fully depressed. - Moore
        } else
        {
            movementBoostScalar = (-20.0f * gamepadTriggerOffset / 2.0f); //Lower range is from -10.0 to 0.0. Because it's additive, if the player isn't moving, the player will move backwards. We can fix this later. - Moore
        }

        //destination = transform.forward * movespeed;
        rigidbody.AddForce((transform.forward * movespeed * gamepadLeftVerticalOffset * (movementScalar + movementBoostScalar)));
        rigidbody.AddForce((transform.right * movespeed / 2 * gamepadLeftHorizontalOffset * (movementScalar + movementBoostScalar)));

        //transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.forward * movespeed * Input.GetAxis("Vertical")), movespeed); //Moves forward based on the vertical axis (Joystick left stick or Keyboard WS keys). - Moore
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.right * movespeed / 2 * Input.GetAxis("Horizontal")), movespeed / 2); //Horizontal speed is half of forward movespeed.
        //The two above commands work, but they work around the physics system. They're good if we're using an object that doesn't need rigid bodies, but isn't going to work as well for TSHE. - Moore

        //if (Input.GetButton("Jump")) {transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.up * movespeed / 2), movespeed / 2);} //Vertical speed is also halved. | Another older version that ignored rigidbody physics.

        if (Input.GetButton("Jump"))
        {
            rigidbody.AddForce((transform.up * movespeed * (movementScalar + movementBoostScalar)));
        }
    }

    public void AddAir(float amount)
    {
        air += amount;
        if (air > airMax)
        {
            air = airMax;
        }
    }

    public void AddAirMax(float amount)
    {
        airMax += amount;
        AddAir(amount);
    }

    public void AddScore(float amount)
    {
        score += amount;
    }
}
