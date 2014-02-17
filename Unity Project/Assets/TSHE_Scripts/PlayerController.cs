using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


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
	public float staminaGainRate = 0.1f;
	public float stamina;

	public float airMax = 100f;
	public float airLossRate = 0.06f;
	public float air;

	public float score = 0f;


	//public Vector3 destination = Vector3.zero;//Deleteme

	//--START OF EVENT METHODS

	// Use this for initialization
	void Start () 
	{
		health = healthMax;
		stamina = staminaMax;
		air = airMax;
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		air -= airLossRate;
		if (air < 0)
		{
			//If the player is out of air, then they rapidly lose health? Or instant game-over? - Moore
			health -= airLossRate *2; //This is assuming drowning drains health gradually. Replace this if it's insta-lose. - Moore
			if (health < 0) //Ditto. - Moore 
			{
				health = 0;
				//TODO: Game Over Here!
			}
			air = 0;
		}

		if (stamina < staminaMax)
		{
			stamina += staminaGainRate;
			if (stamina > staminaMax)
			{
				stamina = staminaMax;
			}
		}
		HandlePlayerInput();

		//rigidbody.velocity = Vector3.zero; - How the frig do we make the thing not fly off when it touches the ground? And not have the terrain fly off either? Either way, we need to stop the passthrough. - Moore
	}

	void OnGUI()
	{
		DrawHUD();
	}

	void OnTriggerEnter(Collider other)
	{
		score += 100;
		air += 10;
		Destroy(other.gameObject); //Again, probably not the best place to put this, but it should work for now. - Moore
	}

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
		//Movement Section
		//destination = transform.forward * movespeed;
		transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.forward * movespeed * Input.GetAxis("Vertical")), movespeed); //Moves forward based on the vertical axis (Joystick left stick or Keyboard WS keys). - Moore
		transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.right * movespeed / 2 * Input.GetAxis("Horizontal")), movespeed / 2); //Horizontal speed is half of forward movespeed.
		if (Input.GetButton("Jump")) {transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.up * movespeed / 2), movespeed / 2);} //Vertical speed is also halved.
	}
}
