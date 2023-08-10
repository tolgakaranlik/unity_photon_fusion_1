using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private CharacterController _controller;
    public Animator Anim;

    public float JumpForce = 2f;
    public float PlayerSpeed = 2f;
    public Vector3 velocity;
    public Camera Camera;
    public float GravityValue = -9.81f;

    //public enum MovementType { Idle, Runnig };
    public const byte MT_IDLE = 0;
    public const byte MT_RUNNING = 1;

    [Networked(OnChanged = nameof(MovementStateChange))] public byte/*MovementType*/ Movement { get; set; }

    public static void MovementStateChange(Changed<PlayerMovement> changed)
    {
        //changed.Behaviour.Anim.Play(changed.Behaviour.Movement == MovementType.Runnig ? "Run" : "Idle");
        changed.Behaviour.Anim.Play(changed.Behaviour.Movement == MT_RUNNING ? "Run" : "Idle");
    }

    bool _jumpPressed = false;

    [Networked] public NetworkButtons Buttons { get; set; }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Only move own player and not every other player. Each player controls its own player object.
        if (HasStateAuthority == false)
        {
            return;
        }

        if (_controller.isGrounded)
        {
            velocity = new Vector3(0, -1, 0);
        }

        if (_jumpPressed && _controller.isGrounded)
        {
            velocity.y += JumpForce;
        }

        var cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;
        velocity.y += GravityValue * Runner.DeltaTime;
        _controller.Move(move + velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
            //Anim.Play("Run");
            Movement = MT_RUNNING; //MovementType.Runnig;
        }
        else
        {
            //Anim.Play("Idle");
            Movement = MT_IDLE; //MovementType.Idle;
        }

        _jumpPressed = false;
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<FirstPersonCamera>().Target = GetComponent<NetworkTransform>().InterpolationTarget;
        }
    }

}