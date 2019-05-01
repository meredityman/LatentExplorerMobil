using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OSCSettings : MonoBehaviour {
    public GameObject oscManager;
    public LatentSliders latentSliders;

    OSC osc;
    GanRunner ganRunner;

    public Button activeMenuButton;

    public InputField recievePortInput;
    public InputField sendPortInput;
    public InputField targetIPInput;
    public Toggle     sendBlindToggle;
    public Slider     rateSlider;
    public Button     resetLatentsButton;


    private bool bActivated;
    RectTransform rectTransform;

	// Use this for initialization
	void Start () {
        osc = oscManager.GetComponent<OSC>();
        ganRunner = oscManager.GetComponent<GanRunner>();

        rectTransform = GetComponent<RectTransform>();

        recievePortInput.text = osc.InPort.ToString("0000");
        sendPortInput.text    = osc.OutPort.ToString("0000");
        targetIPInput.text    = osc.OutIP;
        sendBlindToggle.isOn  = ganRunner.bSendBlind;
        rateSlider.value      = ganRunner.sendRate ;

        SetMenuPosition();
    }

    private void OnEnable()
    {
        activeMenuButton.onClick.AddListener(OnToggleActivated);

        recievePortInput.onEndEdit.AddListener(OnRecievePortEdited);
        sendPortInput.onEndEdit.AddListener(OnSendPortEdited);
        targetIPInput.onEndEdit.AddListener(OnTargetIPEdited);
        sendBlindToggle.onValueChanged.AddListener(OnSendBlindValueChanged);
        rateSlider.onValueChanged.AddListener(OnRateChanged);
        resetLatentsButton.onClick.AddListener(OnNewLatents);
    }

    private void OnDisable()
    {
        activeMenuButton.onClick.RemoveListener(OnToggleActivated);

        recievePortInput.onEndEdit.RemoveListener(OnRecievePortEdited);
        sendPortInput.onEndEdit.RemoveListener(OnSendPortEdited);
        targetIPInput.onEndEdit.RemoveListener(OnTargetIPEdited);
        sendBlindToggle.onValueChanged.RemoveListener(OnSendBlindValueChanged);
        rateSlider.onValueChanged.RemoveListener(OnRateChanged);
        resetLatentsButton.onClick.RemoveListener(OnNewLatents);
    }

    private void SetMenuPosition()
    {
        GetComponent<RectTransform>().position = new Vector3(
            bActivated ? 0 : -rectTransform.rect.width,
            rectTransform.position.y,
            rectTransform.position.z
            );
    }

    public void OnRateChanged(float value)
    {
        ganRunner.sendRate = value;
    }
   
    public void OnNewLatents()
    {
        latentSliders.SetupSliders();
    }

    public void OnToggleActivated()
    {
        bActivated = !bActivated;
        Debug.Log("Here");
        activeMenuButton.GetComponent<ButtonTransformSwitch>().SwitchTransform(bActivated);

        latentSliders.SetActive(!bActivated);

        SetMenuPosition();
    }


    public void OnSendBlindValueChanged(bool newvalue)
    {
        ganRunner.bSendBlind = sendBlindToggle.isOn;
    }

    public void OnRecievePortEdited(string newRecievePort)
    {
        if (ValidatePort(newRecievePort))
        {
            osc.InPort = int.Parse(newRecievePort);
        }
        recievePortInput.text = osc.InPort.ToString("0000");

    }

    public void OnSendPortEdited(string newSendPort)
    {
        if (ValidatePort(newSendPort))
        {
            osc.OutPort = int.Parse(newSendPort);
        }
        sendPortInput.text = osc.OutPort.ToString("0000");

    }

    void OnTargetIPEdited(string newTargetIP)
    {
        if (ValidateIPAddress(newTargetIP)){
            osc.OutIP = targetIPInput.text;
        } else
        {
            targetIPInput.text = osc.OutIP;
        }
    }

    bool ValidateIPAddress( string ipString)
    {
        if (string.IsNullOrEmpty(ipString))
        {
            return false;
        }

        string[] splitValues = ipString.Split('.');
        if (splitValues.Length != 4)
        {
            return false;
        }

        byte tempForParsing;

        return splitValues.All(r => byte.TryParse(r, out tempForParsing));
    }

    bool ValidatePort(  string portString)
    {
        int porttest = 0;
        try
        {
            porttest = int.Parse(portString);
        }
        catch (System.Exception)
        {
        }

        return porttest > 1 && porttest < 65535;
    }
}
