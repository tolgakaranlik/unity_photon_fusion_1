using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON1 = 0x01;

    public byte buttons;
    public Vector3 direction;
    public bool jumpPressed;
}