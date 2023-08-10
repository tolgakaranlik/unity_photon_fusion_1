using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    public int Damage = 10;

    [Networked] private TickTimer life { get; set; }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }

        transform.position += 5 * transform.forward * Runner.DeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HostedPlayer player = collision.gameObject.GetComponent<HostedPlayer>();
        if (player != null)
        {
            Runner.Despawn(Object);

            if (!player.Dead)
            {
                player.RemainingHealth -= Damage;
            }
        }
    }
}
