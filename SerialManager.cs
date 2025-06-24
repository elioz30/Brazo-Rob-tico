using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

public class SerialManager : MonoBehaviour
{
    public string com = "COM9"; // Cambia esto al puerto que uses
    public delegate void SerialEvent(string incomingString);
    public static event SerialEvent WhenReceiveDataCall;

    private bool abort;
    private static SerialPort port;
    private Thread serialThread;
    private SynchronizationContext mainThread;
    private char incomingChar;
    private string incomingString;

    void Start()
    {
        try
        {
            port = new SerialPort(com, 9600);
            port.Open();
            port.DiscardOutBuffer();
            port.DiscardInBuffer();
            port.ReadTimeout = 300;

            Debug.Log($"✅ Conectado exitosamente al puerto {com}");

            mainThread = SynchronizationContext.Current ?? new SynchronizationContext();
            serialThread = new Thread(Receive);

            if (port.IsOpen)
            {
                serialThread.Start();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Error al conectar al puerto {com}: {ex.Message}");
        }
    }

    void Receive()
    {
        while (true)
        {
            if (abort)
            {
                serialThread.Abort();
                break;
            }

            try
            {
                incomingChar = (char)port.ReadChar();
            }
            catch (Exception) { }

            if (!incomingChar.Equals('\n'))
            {
                incomingString += incomingChar;
            }
            else
            {
                mainThread.Send((object state) =>
                {
                    WhenReceiveDataCall?.Invoke(incomingString);
                }, null);

                incomingString = "";
            }
        }
    }

    public static void SendInfo(string infoToSend)
    {
        if (port != null && port.IsOpen)
        {
            port.Write(infoToSend);
            Debug.Log($"➡️ Enviado a Arduino: {infoToSend}");
        }
        else
        {
            Debug.LogError("⚠️ El puerto serial no está abierto o es nulo. Verifica conexión y COM.");
        }
    }

    private void OnApplicationQuit()
    {
        abort = true;
        if (port != null && port.IsOpen)
        {
            port.DiscardOutBuffer();
            port.DiscardInBuffer();
            port.Close();
            Debug.Log("🔌 Puerto serial cerrado.");
        }
    }
}
