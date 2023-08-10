using Fusion;
using TMPro;
using UnityEngine;

public class HostedPlayer : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;
    public Animator Anim;

    public float JumpForce = 2f;
    public float PlayerSpeed = 2f;
    public Camera Camera;
    public float GravityValue = -9.81f;

    public const byte MT_IDLE = 0;
    public const byte MT_RUNNING = 1;
    public int Health = 100;

    Canvas canvas;
    TextMeshProUGUI healthDisplay; 

    // Health Stuff
    [Networked(OnChanged = nameof(RemainingHealthChange))] public int RemainingHealth { get; set; }

    [Networked(OnChanged = nameof(MovementStateChange))] public byte Movement { get; set; }

    [Networked(OnChanged = nameof(DeadStateChange))] public bool Dead { get; set; }

    public static void DeadStateChange(Changed<HostedPlayer> changed)
    {
        Debug.Log("Dead state has been changed to: " + changed.Behaviour.Dead);
        if (changed.Behaviour.Dead)
        {
            changed.Behaviour.Anim.Play("Dead");
        }
    }

    public static void RemainingHealthChange(Changed<HostedPlayer> changed)
    {
        Debug.Log("Remaining health is now: " + changed.Behaviour.RemainingHealth);
        if (changed.Behaviour.RemainingHealth <= 0)
        {
            changed.Behaviour.Die();
        }

        changed.Behaviour.healthDisplay.text = changed.Behaviour.RemainingHealth.ToString();
    }

    public void Die()
    {
        Dead = true;
        Anim.Play("Dead");
    }

    // Ball studd
    [SerializeField] private Ball _prefabBall;
    private Vector3 _forward;
    [Networked] private TickTimer delay { get; set; }

    public static void MovementStateChange(Changed<HostedPlayer> changed)
    {
        //changed.Behaviour.Anim.Play(changed.Behaviour.Movement == MovementType.Runnig ? "Run" : "Idle");
        changed.Behaviour.Anim.Play(changed.Behaviour.Movement == MT_RUNNING ? "Run" : "Idle");
    }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        canvas = transform.Find("Interpolation Target/Canvas").GetComponent<Canvas>();
        healthDisplay = canvas.transform.Find("Health").GetComponent<TextMeshProUGUI>();
    }

    public override void Spawned()
    {
        base.Spawned();

        RemainingHealth = Health;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Dead && GetInput(out NetworkInputData data))
        {
            Vector3 move = 5 * data.direction * Runner.DeltaTime * PlayerSpeed;
            if (data.jumpPressed)
            {
                move.y += GravityValue * Runner.DeltaTime;
            }

            data.direction.Normalize();
            _cc.Move(move);

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

            if (data.direction.sqrMagnitude > 0)
            {
                _forward = data.direction;
            }

            // ball throwing logic
            if (delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_prefabBall,
                    transform.position + _forward + Vector3.up, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<Ball>().Init();
                    });
                }
            }
        }
    }
}