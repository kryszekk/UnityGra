using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    float rotX;
    float rotY;
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    public int jumpFactor = 4;
    public float speed = 3.2f;
    Vector3 movement;
    public CharacterController ctrl;


    //grab
    float ActionDistance = 3.0f;
    public bool grabbingSomething = false;
    public GameObject hand;
    public GameObject grabbedObject;

    public Image Fader;
    public float health = 100.0f;
    public bool isAlive = true;
    public Camera PlayerCamera;
    Rigidbody body;

    AudioSource aSource;
    Vector3 old_osition;
    public float step_size = 1.2f;
    public List<AudioClip> stepAudioClipList = new List<AudioClip>();
    public List<AudioClip> jumpAudioClipList = new List<AudioClip>();

    public AudioSource jump_aSource;
    float oldHeight = 0.0f;

    void Start()
    {
        ctrl = GetComponent<CharacterController>();
        body = GetComponent<Rigidbody>();
        aSource = GetComponent<AudioSource>();

    }


    void Update()
    {
        if (isAlive)
        {
            MouseMovement();
            KeyboardMovement();
            Use();
            PlayJumpMusic();
        }
    }
    public void MouseMovement()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        rotX += mouseY * mouseSensitivity * Time.deltaTime;
        rotY += mouseX * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }
    public void KeyboardMovement()
    {
        if (!ctrl.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;//grawiacja
        }
        if (Input.GetKeyDown(KeyCode.Space) && ctrl.isGrounded)//Skakanie
        {
            Jump(new Vector3(movement.x, jumpFactor, movement.z));
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))//bieganie ON
        {
            speed = 8.0f;
            step_size = 2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))//bieganie OFF
        {
            speed = 3.2f;
            step_size = 1.2f;
        }
        if (ctrl.isGrounded) //ustawienie ruchu
        {
            movement.z = Input.GetAxis("Vertical") * speed * transform.forward.z - Input.GetAxis("Horizontal") * speed * transform.forward.x;
            movement.x = Input.GetAxis("Vertical") * speed * transform.forward.x + Input.GetAxis("Horizontal") * speed * transform.forward.z;
            Steps();
            oldHeight = transform.position.y;
        }

        ctrl.Move(movement * Time.deltaTime);
    }
    public void Jump(Vector3 JumpDirection)
    {
        movement = JumpDirection;
        PlaySound(jumpAudioClipList);
    }


    public void Use()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * ActionDistance, Color.red);
            if (grabbingSomething == false)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position,transform.forward,out hit, ActionDistance))
                {
                    //if you hit cube
                    if (hit.transform.gameObject.GetComponent<Grippable>())
                    {
                        grabbedObject = hit.transform.gameObject;
                        grabbingSomething = true;
                        grabbedObject.BroadcastMessage("Use", hand, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        if (!hit.transform.GetComponent<Button_Big>())//button_big -skrypt
                        {
                            hit.transform.gameObject.BroadcastMessage("Use", null, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (grabbingSomething == true)
            {
                grabbedObject.BroadcastMessage("Use", hand, SendMessageOptions.DontRequireReceiver);
                grabbingSomething = false;
                grabbedObject = null; 
            }
        }
    }
    public void Kill()
    {
        isAlive = false;
        body.isKinematic = false;
        body.drag = 3;
        body.angularDrag = 3;
        body.useGravity = true;
        GetComponent<SphereCollider>().isTrigger = false;
        Debug.Log("Przegrales!");
        this.GetComponentInChildren<gun>().enabled = false;
        Fader.GetComponent<Animator>().Play("fader_black");
    }
    public void TakeDameage(float damage)
    {
        if (isAlive)
        {
            health -= damage;
            Debug.Log("Health:" + health);
            if (health <= 0)
            {
                Kill();
            }
            Fader.GetComponent<Animator>().Play("fader_red");
            PlayerCamera.GetComponent<Animator>().Play("camera_hit");
        }
    }
    public void PlaySound(List<AudioClip> lista)
    {
        aSource.clip = lista[Random.Range(0, lista.Count)];
        aSource.Play();
    }
    public void Steps()
    {
        if (Vector3.Distance(transform.position, old_osition) > step_size)
        {
            PlaySound(stepAudioClipList);
            old_osition = transform.position;
            PlayerCamera.GetComponent<Animator>().Play("camera_walk", 0, 0);
        }
    }
    public void PlayJumpMusic()
    {
        float height = transform.position.y - oldHeight;
        jump_aSource.volume = height * height / 50.0f;
    }
}