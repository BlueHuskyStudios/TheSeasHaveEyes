﻿using UnityEngine;
using System.Collections;

public enum ControlStyle
{
    Basic,
    Normal,
    Advanced
}

public class PlayerController : MonoBehaviour
{


    #region Declarations

    //--Constants and other variables that shouldn't be changed during runtime in the final product.
    public float movespeed; //This is set in the inspector to determine the forward-movement speed of the character. - Moore
    public float maxspeed;
    public float strokeDelayScalar;
    public AudioSource swimSoundPlayer;
    public AudioClip swimSound;

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
    public float strokeCooldown; //This is used to track if the player's able to give another 'stroke' in the direction they're facing. - Moore
    public float timeBetweenStrokes; //Ideally, this would be a const, but is public to allow tweaking in the inspector. This is the cooldown is set to each stroke. - Moore

    public float hurtEffectCountdown = 0f; //When the player takes damage, this is set to some value between 0 and 1 inclusive. 0 means 'don't show damage overlay'. 1 means 'do show it'. 


    //Options and Configuration type thigies.
    public ControlStyle controlStyle = ControlStyle.Normal;

    //Declare delegates and events.
    public static event System.Action<float> AddAirEvent; // - Moore

    //Values only for debugging and testing purposes.
    public GameObject lookingAt;

    #endregion Declarations

    #region Unity Event Methods.

    // Use this for initialization
    void Start()
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Player");
        if (allObjects.Length > 1)
        {
            print("WARNING: Attempted to add multiple players to game. Destroying a player...");
            Destroy(gameObject);
        } 

        else
        {
            // Singleton pattern: We only want one player.  If the level starts with more than one, get rid of them.
            health = healthMax;
            stamina = staminaMax;
            air = airMax;

            //And setting up event listener thingies here.
            PickupBubble.CollidedWithPlayer += AddAir;
            PickupBubble.CollidedWithPlayer += AddScore;

            timeBetweenStrokes = 1.0f;
            strokeDelayScalar = timeBetweenStrokes * 15;
        }
    
    }

    void Update()
    {
        hurtEffectCountdown = UpdateCountdownMinMax(hurtEffectCountdown, 0f, 1f);

        strokeCooldown -= Time.deltaTime;
        air -= airLossRate * Time.deltaTime;
    }
    
    // FixedUpdate is called reguarlly and is mainly used to deal with physics. For smoother, quicker changes, use Update.
    void FixedUpdate()
    {


        HandlePlayerInput();


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
        DrawRedOverlay(hurtEffectCountdown);
        DrawHUD();
        //gameObject.GetComponent<3DText>(); //Deleteme?

        GUI.Label(new Rect(0, 0, 100, 100), "Score: " + score.ToString());


    }
    #endregion Unity Event Methods.

    #region Logic Methods.
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

    void DrawRedOverlay(float alpha)
    {
        if (alpha > 1)
        {
            alpha = 1;
        }

        if (alpha > 0)
        {


            Color redShade = new Color(1, 0, 0, alpha);

            // This bit, I had to look up, because I couldn't just set GUI.colors directly. It was more than a little annoying. - Moore.
            Texture2D myTexture = new Texture2D(1, 1);
            myTexture.SetPixel(1, 1, redShade);
            myTexture.wrapMode = TextureWrapMode.Repeat;
            myTexture.Apply();
            //End of looked-up section.

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), myTexture);
        }
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

        //Misc Options Section
        if (Input.GetButtonDown("Fire2"))
        {
            //If the player just used Fire2 (Middleclick) change the movement control style to the next level up. This may be better suited to an on screen button or options menu.
            switch (controlStyle)
            {
            case ControlStyle.Advanced:
                controlStyle = ControlStyle.Basic;
                break;
            case ControlStyle.Basic:
                controlStyle = ControlStyle.Normal;
                break;
            case ControlStyle.Normal:
                controlStyle = ControlStyle.Advanced;
                break;
                
            default:
                break;
            }
        }

        //Movement Section

        gamepadLeftHorizontalOffset = Input.GetAxis("Horizontal");
        gamepadLeftVerticalOffset = Input.GetAxis("Vertical");
        
        gamepadRightHorizontalOffset = Input.GetAxis("RightHorizontal");
        gamepadRightVerticalOffset = Input.GetAxis("RightVertical");

        if (controlStyle == ControlStyle.Basic)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce((transform.forward * movespeed * gamepadLeftVerticalOffset * (movementScalar + movementBoostScalar) * strokeDelayScalar));
            rigidbody.AddForce((transform.right * movespeed / 2 * gamepadLeftHorizontalOffset * (movementScalar + movementBoostScalar) * strokeDelayScalar));

            if (Input.GetButton("Jump"))
            {
                rigidbody.AddForce((transform.up * movespeed * (movementScalar + movementBoostScalar) * strokeDelayScalar));
            }
        } else if (CanStroke())
        {
            bool didStroke = false;
            gamepadTriggerOffset = Input.GetAxis("Trigger");
            if (gamepadTriggerOffset >= 0)
            {
                movementBoostScalar = (20.0f * gamepadTriggerOffset); //Makes the modifier range from 0.0 to 20.0 assuming the trigger is fully depressed. - Moore
            } else
            {
                movementBoostScalar = (-20.0f * gamepadTriggerOffset / 2.0f); //Lower range is from -10.0 to 0.0. Because it's additive, if the player isn't moving, the player will move backwards. We can fix this later. - Moore
            }

            //destination = transform.forward * movespeed;
            if (controlStyle == ControlStyle.Advanced)
            {
                rigidbody.AddForce(new Vector3(-rigidbody.velocity.x, 0, -rigidbody.velocity.z)); //Cancel out existing forces except for Gravity and whatnot.
            } else if (controlStyle == ControlStyle.Normal)
            {
                rigidbody.AddForce(-rigidbody.velocity); //Cancel out all forces.
            } 
            rigidbody.AddForce((transform.forward * movespeed * gamepadLeftVerticalOffset * (movementScalar + movementBoostScalar) * strokeDelayScalar));
            rigidbody.AddForce((transform.right * movespeed / 2 * gamepadLeftHorizontalOffset * (movementScalar + movementBoostScalar) * strokeDelayScalar));

            if (gamepadLeftVerticalOffset != 0 || gamepadLeftHorizontalOffset != 0)
            {
                didStroke = true;
            }

            //transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.forward * movespeed * Input.GetAxis("Vertical")), movespeed); //Moves forward based on the vertical axis (Joystick left stick or Keyboard WS keys). - Moore
            //transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.right * movespeed / 2 * Input.GetAxis("Horizontal")), movespeed / 2); //Horizontal speed is half of forward movespeed.
            //The two above commands work, but they work around the physics system. They're good if we're using an object that doesn't need rigid bodies, but isn't going to work as well for TSHE. - Moore

            //if (Input.GetButton("Jump")) {transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.up * movespeed / 2), movespeed / 2);} //Vertical speed is also halved. | Another older version that ignored rigidbody physics.

            if (Input.GetButton("Jump"))
            {
                rigidbody.AddForce((transform.up * movespeed * (movementScalar + movementBoostScalar) * strokeDelayScalar));
                didStroke = true;
            }

            if (didStroke)
            {
                ResetStrokeCooldown();
                if (movementBoostScalar != 0)
                {
                    strokeCooldown /= 2;
                } //Boost means twice as many strokes per unit time and twice the speed.
            }

            IsLookingAt();
        }

        //Enforce a maximum speed.
        if (rigidbody.velocity.magnitude > maxspeed)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * maxspeed;
        }

        //TODO: Allow the player to attack. This doesn't matter if the player has used a stroke or not.
    }

    void GameOver()
    {
        //STUB
        print("Oh noes, you Game Overed! Have some free HP.");
        Time.timeScale = 0f;
        //AddHealth(9999f);

    }

    #endregion Logic Methods

    #region Mutator Methods
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health < 0)
        {
            health = 0;
            GameOver();
        }

        hurtEffectCountdown = 1f;
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

    public void AddHealth(float amount)
    {
        health += amount;
        if (health > healthMax)
        {
            health = healthMax;
        }
    }
    
    public void AddHealthMax(float amount)
    {
        healthMax += amount;
        AddHealth(amount);
    }

    public void AddScore(float amount)
    {
        score += amount;
    }
    #endregion Mutator methods

    #region Utility Methods.
    protected bool CanStroke()
    {
        bool result = false;
        if (strokeCooldown <= 0)
        {
            result = true;
        }
        return result;
    }

    protected void ResetStrokeCooldown()
    {
        if (swimSound != null && swimSoundPlayer != null)
        {
            swimSoundPlayer.PlayOneShot(swimSound, 1);
        }
        strokeCooldown = timeBetweenStrokes;
    }

    //Casts a ray. If the ray hits a player, then return true. Else, return false.
    public bool IsLookingAt(float distance)
    {
        bool result = false;
        
        RaycastHit theHit;
        if (Physics.Raycast(transform.position, transform.forward, out theHit, distance))
        {
            GameObject theGameObject = theHit.collider.gameObject;
            lookingAt = theGameObject;
            if (theGameObject != null)
            {
                result = true;
            }
            if (GameController.Testing)
            {
                //If we want the player to display testing data, here might be a spot.
            }
        } 
        
        return result;
    }
    
    //Convenience method for the above: Just assumes a distance of 1k
    public bool IsLookingAt()
    {
        return IsLookingAt(Mathf.Infinity);
    }

    //Usage: For accurate results, only call this once per Update per countdownVariable. - Moore
    float UpdateCountdownMinMax(float countdown, float min, float max)
    {
        float result = countdown;

        //Clamp to upper bounds.
        if (countdown > max)
        {
            countdown = max;
        }

        //Clamp to lower bounds.
        if (countdown < min)
        {
            countdown = min;
        } 
        //Decrement when not under bounds.
        else if (countdown > min)
        {
            result -= Time.deltaTime;
        }
        return result;
    }

    #endregion Utility Methods.
}
