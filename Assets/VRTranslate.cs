using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;

public class VRTranslate : MonoBehaviour
{
    public float translate_gain = 1.0f; 
    public float rotation_gain = 1.0f; 
    public float curvature_gain = 1.0f; 

    public float m_speed = 0.01f; 
    public float r_speed = 0.1f; 
    GameObject playerCam;        
    GameObject playerReal;       
    public float mouseSensitivity = 100f; 
    private Vector3 initialCamPosition; 
    public Vector3 cameraOffset = new Vector3(0, 0.8f, 0);
    private Quaternion initialCamRotation; 
    private Vector3 initialPlayerPosition;

    private InputAction moveAction;
    private InputAction lookAction;
    private DualShockGamepad dualShockGamepad;
private float currentPitch = 0f;
    void Start()
    {
        playerCam = GameObject.Find("PlayerCam");
        playerReal = GameObject.Find("PlayerReal");

        initialCamPosition = playerCam.transform.position;
        initialCamRotation = playerCam.transform.rotation;
        initialPlayerPosition = playerReal.transform.position;

        Debug.Log(Gamepad.current);
        if (Gamepad.current is DualShockGamepad dsGamepad)
        {
            dualShockGamepad = dsGamepad;

            moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            moveAction.Enable();

            lookAction = new InputAction("Look", InputActionType.Value, "<Gamepad>/rightStick");
            lookAction.Enable();
        }
    }

  void Update()
{
    HandleControllerInput();
    //HandleMouseRotation();
    smartCamDisplace();
    /*Vector3 cameraForward = playerCam.transform.forward; 
    Vector3 cameraRight = playerCam.transform.right; 

    cameraForward.y = 0;
    cameraRight.y = 0;

    // Normaliser pour éviter des problèmes d'intensité
    cameraForward.Normalize();
    cameraRight.Normalize();

    if (Input.GetKey(KeyCode.W))
    {
        smartCamDisplace();
        playerReal.transform.position += cameraForward * m_speed;
    }
    if (Input.GetKey(KeyCode.S))
    {
        smartCamDisplace();
        playerReal.transform.position -= cameraForward * m_speed;
    }
    if (Input.GetKey(KeyCode.A))
    {
        smartCamDisplace();
        playerReal.transform.position -= cameraRight * m_speed;
    }
    if (Input.GetKey(KeyCode.D))
    {
        smartCamDisplace();
        playerReal.transform.position += cameraRight * m_speed;
    }*/
}

void HandleControllerInput()
{
        if (dualShockGamepad == null) return;

        // Lecture des entrées du joystick
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // Calcul des vecteurs avant et droite de la caméra
        Vector3 cameraForward = playerCam.transform.forward;
        Vector3 cameraRight = playerCam.transform.right;

        // Éliminer les composantes verticales pour le mouvement
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normaliser pour un déplacement cohérent
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Déplacement du joueur basé sur l'entrée du stick gauche
        Vector3 moveDirection = (cameraRight * moveInput.x + cameraForward * moveInput.y) * m_speed * translate_gain;
        playerReal.transform.position += moveDirection;

        // Rotation horizontale (yaw) - Rotation du playerReal
        float yawRotation = lookInput.x * r_speed * rotation_gain;
        playerReal.transform.Rotate(Vector3.up * yawRotation);

        // Rotation verticale (pitch) - Mise à jour de currentPitch
        float pitchDelta = -lookInput.y * r_speed * rotation_gain;
        currentPitch = Mathf.Clamp(currentPitch + pitchDelta, -90f, 90f);
    }
void HandleMouseRotation()
{
    // Rotation horizontale (gauche/droite)
     float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * rotation_gain;
    playerReal.transform.Rotate(Vector3.up * mouseX);

   

    smartCamDisplace();
}


    void smartCamDisplace()
    {
        translateCam();
        rotateCam();
       // curveCam();
    }

   void translateCam()
{
    // Synchronisation entre *playerReal* et la caméra
  playerCam.transform.position = playerReal.transform.position + cameraOffset;
}

void rotateCam()
{
Quaternion horizontalRotation = playerReal.transform.rotation;
        
        // Ajout de la rotation verticale
        Quaternion verticalRotation = Quaternion.Euler(currentPitch, 0, 0);
        
        // Combine les rotations : d'abord la rotation horizontale, puis la verticale
        playerCam.transform.rotation = horizontalRotation * verticalRotation;
}


    void curveCam()
    {
        float R = 22f;
        float curvature_gain = 1f / R;

        Vector3 movementDirection = playerReal.transform.forward * m_speed;
        Vector3 perpendicularDirection = Vector3.Cross(movementDirection, Vector3.up).normalized;
        Vector3 curvedMovement = movementDirection + perpendicularDirection * curvature_gain;

        playerCam.transform.position += curvedMovement;
    }
}
