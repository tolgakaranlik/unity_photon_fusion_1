using Fusion;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    public MeshRenderer MeshRenderer;

    [Networked(OnChanged = nameof(NetworkColorChanged))]
    public Color NetworkedColor { get; set; }

    private static void NetworkColorChanged(Changed<PlayerColor> changed)
    {
        // ezberleyin: doğrudan NetworkBehavior üstündeki nesnelere buradan erişmeye TEŞEBBÜS ETMEYİN!
        changed.Behaviour.MeshRenderer.material.color = changed.Behaviour.NetworkedColor;

        // thread safety
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }
}