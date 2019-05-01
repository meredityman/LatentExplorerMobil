using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;




public class LatentSliders : MonoBehaviour {

    public static LatentSliders instance;

    public static int numSliders = 20;
    public GameObject sliderPrefab;
    public GanRunner ganRunner;

    private List<Slider> sliders;
    RectTransform rectTransform;
    // Use this for initialization

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("Something is wrong");
        }

    }

    void Start () {

        rectTransform = GetComponent<RectTransform>();

        SetupSliders();
    }

    public static float GetSliderValue(int i)
    {
        if(instance == null)
        {
            Debug.LogWarning("Something is wrong");
            return 0.0f;
        } else
        {
            return instance.sliders[i].value;
        }
               
    }

    public void SetupSliders()
    {
        if(sliders != null)
        {
            foreach (var slider in sliders)
            {
                Destroy(slider.gameObject);
            }
        }

        sliders = new List<Slider>();

        IEnumerable<int> latentIndecies = Enumerable.Range(0, GANData.numLatents - 1).Randomize().Take(numSliders);

        foreach (var it in latentIndecies.Select((Value, Index) => new { Value, Index }))
        {

            Vector2 position = new Vector2(
                ((rectTransform.rect.width / ((float)numSliders) * (it.Index + 0.5f))) - (rectTransform.rect.width * 0.5f),
                0.0f
            );

            GameObject newSliderGo = Instantiate(sliderPrefab, rectTransform);
            newSliderGo.name = "Latent_" + it.Value.ToString();
            newSliderGo.SetActive(true);


            RectTransform newSliderTransfrom = newSliderGo.GetComponent<RectTransform>();

            newSliderTransfrom.anchoredPosition = position;
            newSliderTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height);

            Slider newSlider = newSliderGo.GetComponent<Slider>();

            int index = it.Value;
            newSlider.onValueChanged.AddListener((val) => OnLatentChanged(index, val));

            sliders.Add(newSlider);
        }
    }

    public void SetActive(bool bActive)
    {
        foreach( var slider in sliders)
        {
            slider.interactable = bActive;
        }
    }

    void OnLatentChanged( int index, float value)
    {
        Debug.Log(index + " changed to " + value);

        ganRunner.UpdateLatent(index, value);
    }

}


static class Extensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        System.Random rnd = new System.Random();
        return source.OrderBy<T, int>((item) => rnd.Next());
    }
}