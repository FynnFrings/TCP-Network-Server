using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Attributes
    //Joystick joystick;

    // Start is called before the first frame update
    void Start()
    {
        //joystick = GameObject.FindWithTag("Joystick").GetComponent<Joystick>();
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D),
            Input.GetMouseButton(0)
        };

        ClientSend.PlayerMovement(_inputs);

        //MousePosition
        Vector3 _playerMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _playerMousePosition.z = 0;

        ClientSend.PlayerMousePosition(_playerMousePosition);
    }
}
