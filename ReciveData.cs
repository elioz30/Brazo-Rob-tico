using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveDataScript : MonoBehaviour
{
    [Header("Datos recibidos")]
    public string dataIn;
    public float rotationAng;
    public float x;
    public float speed;

    [Header("Referencias a partes del modelo (pivotes de rotación)")]
    public Transform basePivot;    // Pivote-00 → rotación en Y
    public Transform brazo1Pivot;  // Pivote-01 → rotación en X
    public Transform brazo2Pivot;  // Pivote-02 → rotación en X

    void Start()
    {
        SerialManager.WhenReceiveDataCall += ReceiveData;
    }

    void ReceiveData(string incomingString)
    {
        dataIn = incomingString.Trim();

        string[] partes = dataIn.Split(',');

        if (partes.Length == 3)
        {
            // Modo gemelo digital: recibe 3 ángulos
            if (int.TryParse(partes[0], out int baseAng) &&
                int.TryParse(partes[1], out int brazo1Ang) &&
                int.TryParse(partes[2], out int brazo2Ang))
            {
                basePivot.localRotation = Quaternion.Euler(0, baseAng, 0);
                brazo1Pivot.localRotation = Quaternion.Euler(brazo1Ang, 0, 0);
                brazo2Pivot.localRotation = Quaternion.Euler(brazo2Ang, 0, 0);
            }
        }
        else
        {
            // Modo antiguo (1 valor): transforma en rotación simple
            if (float.TryParse(dataIn, out x))
            {
                x = x / 100;
                rotationAng = Mathf.Lerp(0, 360, x / 1023f);
            }
        }
    }

    void Update()
    {
        // Controles por teclado
        if (Input.GetKeyDown(KeyCode.A)) SerialManager.SendInfo("a"); // base a 180°
        if (Input.GetKeyDown(KeyCode.B)) SerialManager.SendInfo("b"); // base a 0°
        if (Input.GetKeyDown(KeyCode.C)) SerialManager.SendInfo("c"); // brazo 1 y 2 a 90°
        if (Input.GetKeyDown(KeyCode.D)) SerialManager.SendInfo("d"); // brazo 1 y 2 a 0°
        if (Input.GetKeyDown(KeyCode.R)) SerialManager.SendInfo("r"); // reset

        // Controles por joystick
        if (Input.GetKeyDown("joystick button 0")) SerialManager.SendInfo("a");
        if (Input.GetKeyDown("joystick button 1")) SerialManager.SendInfo("b");
        if (Input.GetKeyDown("joystick button 3")) SerialManager.SendInfo("c");
        if (Input.GetKeyDown("joystick button 4")) SerialManager.SendInfo("d");
        if (Input.GetKeyDown("joystick button 7")) SerialManager.SendInfo("r");
    }

    void FixedUpdate()
    {
        // Si se recibe solo un valor, rota este GameObject principal (modo antiguo)
        transform.rotation = Quaternion.Euler(rotationAng, 0, 0);
    }
}
