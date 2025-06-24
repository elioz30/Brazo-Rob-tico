using UnityEngine;

public class BotonPrueba : MonoBehaviour
{
    void Update()
    {
        for (int i = 0; i < 20; i++) // prueba hasta 20 botones
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                Debug.Log("Presionado botón: " + i);
            }
        }
    }
}
