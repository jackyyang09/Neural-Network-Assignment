using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Text generationCounter;
    int generations;

    [SerializeField]
    Camera globalCam;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }
    
    public void SubscribeToEvents()
    {
        NeuralNetMaster.Instance.onReset += IncrementGenerationCounter;
    }

    void IncrementGenerationCounter()
    {
        generations++;
        generationCounter.text = generations.ToString();
    }

    public void ToggleGlobalCam()
    {
        globalCam.enabled = !globalCam.enabled;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
