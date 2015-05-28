using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


[RequireComponent(typeof(NetworkView))]
public class InputHandler : MonoBehaviour
{
	
	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;
	private GameServer m_ServerRef;
	
	private Quaternion m_Look;
	
	[SerializeField]
	private bool
		m_IsWalking;

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("start");
		m_Look = Quaternion.identity;
	}

	private void Update ()
	{
		GetLook ();
	}
	private void FixedUpdate ()
	{
		if (Network.isClient) {
			GetInput ();
		}
	}


	private void GetInput ()
	{
		Debug.Log ("input");
		// Read input
		float horizontal = CrossPlatformInputManager.GetAxis ("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis ("Vertical");

		#if !MOBILE_INPUT
		// On standalone builds, walk/run speed is modified by a key press.
		// keep track of whether or not the character is walking or running
		bool walking = !Input.GetKey (KeyCode.LeftShift);
		#endif
		int walkingInt = 0;
		if (walking) {
			walkingInt = 1;
		}
		Vector3 input = new Vector3 (horizontal, vertical, walkingInt);
		GetComponent<NetworkView> ().RPC ("TransmitInput", RPCMode.Server, input, m_Look);
	}

	public void GetLook ()
	{
		float yRot = CrossPlatformInputManager.GetAxis ("Mouse X") * XSensitivity;
		float xRot = CrossPlatformInputManager.GetAxis ("Mouse Y") * YSensitivity;
		
		m_Look *= Quaternion.Euler (-xRot, yRot, 0f);

		if (clampVerticalRotation)
			m_Look = ClampRotationAroundXAxis (m_Look);
	}

	Quaternion ClampRotationAroundXAxis (Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}

	[RPC]
	void TransmitInput (Vector3 input, Quaternion look, NetworkMessageInfo info)
	{
		if (Network.isServer) {
			if (m_ServerRef == null) {
				m_ServerRef = GameObject.FindGameObjectWithTag ("GameServer").GetComponent<GameServer> ();
			}
			m_ServerRef.GetPlayer (info.sender).GetComponent<FPSPlayer> ().SetInput (input, look);
			//	Debug.Log ("input " + input + " info " + info.sender.ToString ());
		} else {
			//	Debug.Log ("SOMETHIGN TERRIBLE HAS HAPPENED ON THE NETWORK");
		}
	}
}
