using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GANData
{
    public const int numLatents = 512;
    public const int numLayers = 18;
    public bool hasChanged { get {return _hasChanged;}}

    private bool _hasChanged = true;

    public float getLatent(int i) { return latents[i]; }
    public float[] getLatents() { return latents; }


    public GANData()
    {
        Reset();
    }


    public void SetLatent(int index, float val)
    {
        latents[index] = val;
        _hasChanged = true;
    }


    public void markSent() { _hasChanged = false; }

    public void Reset()
    {
        latents  = new float[numLatents];
        dlatents = new float[numLayers, numLatents];
        _hasChanged = true;
    }


    private float[] latents = new float[numLatents];
    private float[,] dlatents = new float[numLayers, numLatents];
}


public class GanRunner : MonoBehaviour {

    public bool bPing = true;

    public bool bSendBlind = true;
    public float sendRate = 10.0f;

    OSC osc;

    GANData ganData;

    public void UpdateLatent( int index, float val)
    {
        ganData.SetLatent(index, val);
    }

    private bool _processed = true;

	// Use this for initialization
	void Start () {
        osc = GetComponent<OSC>();

        osc.SetAllMessageHandler(AllMessageHandler);

        ganData = new GANData();

        StartCoroutine("Ping");
        StartCoroutine("SendLoop");
	}
	

    IEnumerator SendLoop()
    {
        while (true)
        {
            if (bSendBlind)
            {
                Send(true);
                yield return new WaitForSeconds( 1.0f / sendRate );
            } else
            {
                if (ganData.hasChanged && _processed)
                    Send();

                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator Ping()
    {
        while (true)
        {
            if (bPing)
            {
                OscMessage message = new OscMessage();
                message.address = "/ping/";
                message.values.Add(1);
                osc.Send(message);
                Debug.Log("Ping Sent");
                
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    void AllMessageHandler(OscMessage message)
    {
        Debug.Log(message.ToString());

        var address = message.address.Split('/');
        if (address[1] == "processed") {
            _processed = true;
        };
    }


    void Send(bool tagExtern = false)
    {
        OscMessage message = new OscMessage();
        message.address = tagExtern ? "/externLatents/" : "/latents/";
        for (int i = 0; i < GANData.numLatents; i++) { message.values.Add(ganData.getLatent(i)); }

        osc.Send(message);
        ganData.markSent();

        if(!tagExtern)
            Process();
    }

    void Process()
    {
        OscMessage message = new OscMessage();
        message.address = "/process/";
        message.values.Add(1);
        osc.Send(message);
        _processed = false; ;
    }
}
