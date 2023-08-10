using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;

public class InputManager : MonoBehaviour
{
    bool alpha1Pressed = false;
    bool alpha2Pressed = false;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        /*var myInput = new PlayerInputs();

        myInput.buttons.Set(PlayerButtons.DisplayDialog1, alpha1Pressed);
        myInput.buttons.Set(PlayerButtons.DisplayDialog2, alpha2Pressed);

        input.Set(myInput);
        input = default;*/
    }

    private void Update()
    {
        alpha1Pressed = Input.GetKey(KeyCode.Alpha1);
        alpha2Pressed = Input.GetKey(KeyCode.Alpha2);
    }
}
