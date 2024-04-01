using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class ConnectBluetoothFSR : MonoBehaviour
{
    private BluetoothHelper helper;
    public GameObject connectBtn;
    public GameObject disconnectBtn;    
    public GameObject led1;
    public GameObject led2;
    public TextMeshProUGUI simpleUIText;
    public TextMeshProUGUI Text_FSR1;
    public TextMeshProUGUI Text_FSR2;
    public int fsr1;
    public int fsr2;



    void Awake()
    {

        BluetoothHelper.BLE = false;
        helper = BluetoothHelper.GetInstance();
        helper.OnConnected += OnConnected;
        helper.OnConnectionFailed += OnConnectionFailed;
        helper.OnDataReceived += OnDataReceived;
        //helper.setFixedLengthBasedStream(1); //data is received byte by byte
        helper.setTerminatorBasedStream("\n"); //every messages ends with new line character
        helper.setDeviceName("HC-05");


    }

    void OnConnected(BluetoothHelper helper)
    {
        helper.StartListening();
        connectBtn.SetActive(false);
        disconnectBtn.SetActive(true);
        helper.SendData("0");
        simpleUIText.text = "Connected";
    }

    void OnConnectionFailed(BluetoothHelper helper)
    {
        Debug.Log("FSR Failed to connect");
        simpleUIText.text = "Failed to connect";
        connectBtn.SetActive(true);
        disconnectBtn.SetActive(false);
    }

    void OnDataReceived(BluetoothHelper helper)
    {
        string msg = helper.Read();        
        //simpleUIText.text = msg;
        string[] temp = msg.Split(',');
        Text_FSR1.text = temp[1];
        Text_FSR2.text = temp[0];

        /*  //This is for LED
        switch (msg)
        {
            case "1":
                led.GetComponent<Renderer>().material.color = Color.red;
                break;
            case "0":
                led.GetComponent<Renderer>().material.color = Color.gray;
                break;
            default:
                Debug.Log($"Received unknown message [{msg}]");
                break;
        }
        */
        fsr1 = int.Parse(temp[1]);
        fsr2 = int.Parse(temp[0]);
        //Debug.Log("woan ning:" + fsr1 + "," + fsr2);

        //set the LED color
        if (fsr1 > 100)
        {
            led1.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            led1.GetComponent<Renderer>().material.color = Color.white;
        }

        if (fsr2 > 100)
        {
            led2.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            led2.GetComponent<Renderer>().material.color = Color.white;
        }


        //double tempfsr = (double) fsr1 / 1023 * 255;
        //Debug.Log("woan ning: set to tempfsr" + tempfsr);
        //int color = (int) ((double)fsr1 / 1023 * 255);
        //GameObject.Find("Cube3").GetComponent<Renderer>().material.color = new Color(0, 0, color);
        //Debug.Log("woan ning: set to " + color);

        /*
        if (fsr1 > 600)
        {
            //cube.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            GameObject.Find("Cube3").GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            //Debug.Log("woan ning: set to red");
        }
        else if (fsr1 > 300)
        {
            //cube.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            GameObject.Find("Cube3").GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            //Debug.Log("woan ning: set to green");
        }
        else if (fsr1 > 100)
        {
            //cube.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
            GameObject.Find("Cube3").GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            //Debug.Log("woan ning: set to blue");
        }
        */


    }

    public void Connect()
    {
        helper.Connect();
    }

    public void Disconnect()
    {
        helper.Disconnect();
        connectBtn.SetActive(true);
        disconnectBtn.SetActive(false);
        simpleUIText.text = "Disconnected";
    }

    public void sendData(string d)
    {
        helper.SendData(d);
        simpleUIText.text = "send data:" + d;
    }

    public void Update()
    {
       
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BluetoothHelper.BLE = false;
        helper = BluetoothHelper.GetInstance();
        helper.OnConnected += OnConnected;
        helper.OnConnectionFailed += OnConnectionFailed;
        helper.OnDataReceived += OnDataReceived;
        //helper.setFixedLengthBasedStream(1); //data is received byte by byte
        helper.setTerminatorBasedStream("\n"); //every messages ends with new line character
        helper.setDeviceName("HC-05");
    }
}
