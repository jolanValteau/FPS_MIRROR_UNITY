using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(Animator))]

public class PlayerControlleur : MonoBehaviour
{
	[Header("Player Settings")]
	[SerializeField]
	private float speed;
	
	[SerializeField]
	private float mouseSensitivityX;
	[SerializeField]
	private float mouseSensitivityY;
	
	[SerializeField]
	private float thrusterForce;
	
	[SerializeField]
	private float thrusterFuelBurnSpeed;
	[SerializeField]
	private float thrusterFuelRegenSpeed;
	private float thrusterFuelAmount = 1f;
	
	public float GetThrusterFuelAmount()
	{
		return thrusterFuelAmount;
	}
	
	[Header("Joint Options")]
	[SerializeField]
	private float jointSpring;
	[SerializeField]
	private float jointMaxForce;
	
	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;

	void Awake()
	{
        speed = 5f;
        mouseSensitivityX = 7f;
        mouseSensitivityY = 7f;
        jointMaxForce = 50f;
		jointSpring = 20f;
		thrusterFuelBurnSpeed = 0.5f;
		thrusterFuelRegenSpeed = 1f;
		thrusterFuelAmount = 1f;
		thrusterForce = 1000f;
}

    private void Start()
	{
		motor = GetComponent<PlayerMotor>();
		joint = GetComponent<ConfigurableJoint>();
		animator = GetComponent<Animator>();
		SetJointSettings(jointSpring);
	}
	
	private void Update()
	{
		if(PauseMenu.isOn)
		{
		if(Cursor.lockState != CursorLockMode.None)
		{
			Cursor.lockState = CursorLockMode.None;
		}
					
			motor.Move(Vector3.zero);
			motor.Rotate(Vector3.zero);
			motor.RotateCamera(0f);
			motor.ApplyThruster(Vector3.zero);
			return;
		}
		
		if(Cursor.lockState != CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
		
		RaycastHit _hit;
		if(Physics.Raycast(transform.position, Vector3.down, out _hit, 100f))
		{
			joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
		}
		else{
			joint.targetPosition = new Vector3(0f, 0f, 0f);
		}
		// Vitesse.mouvement.joueur
		
		// Detect.Data
		float xMov = Input.GetAxis("Horizontal");
		float zMov = Input.GetAxis("Vertical");
		
		// Calc.Move
		Vector3 moveHorizontal = transform.right * xMov;
		Vector3 moveVertical = transform.forward * zMov;
		
		// Calc.Velocity.Total
		Vector3 velocity = (moveHorizontal + moveVertical) * speed;
		
		//Animation
		animator.SetFloat("ForwardVelocity", zMov);
		
		motor.Move(velocity);
		
		//Calc.yRotate
		float yRot = Input.GetAxisRaw("Mouse X");
		
		Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;
		
		motor.Rotate(rotation);
		
		//Calc.xRotate
		float xRot = Input.GetAxisRaw("Mouse Y");
		
		float cameraRotationX = xRot * mouseSensitivityY;
		
		motor.RotateCamera(cameraRotationX);
		
		//Calc.Var'thrusterVelocity
		Vector3 thrusterVelocity = Vector3.zero;
		if(Input.GetButton("Jump") && thrusterFuelAmount > 0)
		{
			thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;
			
			if(thrusterFuelAmount >= 0.01f)
			{
			thrusterVelocity = Vector3.up * thrusterForce;
			SetJointSettings(0f);
			}
		}
		else
		{
			thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
			SetJointSettings(jointSpring);
		}

		thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

		//Apply.Var'thrusterVelocity
		motor.ApplyThruster(thrusterVelocity);
	}
	
	private void SetJointSettings(float _jointSpring)
	{
		joint.yDrive  = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce};
	}
	
}