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
    RawImage globalCam;

    [SerializeField]
    List<Image> inputNodeImages;

    [SerializeField]
    List<Image> hiddenNodeImages;
    [SerializeField]
    List<ConnectionVisualizer> hiddenConnectors;

    [SerializeField]
    List<Image> outputNodeImages;
    [SerializeField]
    List<ConnectionVisualizer> outputConnectors;

    [SerializeField]
    Text mostFit;

    [SerializeField]
    Text simSpeed;

    //[SerializeField]
    //LineRenderer 

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }
    
    public void SubscribeToEvents()
    {
        NeuralNetMaster.Instance.onReset += IncrementGenerationCounter;
        //NeuralNetMaster.Instance.onReset += UpdateNetworkVisualizer;
    }

    private void Update()
    {
        mostFit.text = NeuralNetMaster.Instance.GetSelectedCar().name;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale = Mathf.Repeat(Time.timeScale + 1, 11);
            simSpeed.text = "Simulation Speed: " + Time.timeScale.ToString();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale = Mathf.Repeat(Time.timeScale - 1, 11);
            simSpeed.text = "Simulation Speed: " + Time.timeScale.ToString();
        }

        UpdateNetworkVisualizer();
    }

    void UpdateNetworkVisualizer()
    {
        NeuralNetwork fit = NeuralNetMaster.Instance.GetSelectedCar();

        List<Neuron> inputNodes = fit.GetInputNodes();
        for (int i = 0; i < inputNodes.Count; i++)
        {
            inputNodeImages[i].color = (inputNodes[i].Activated()) ? Color.green : Color.white;
        }

        List<Neuron> hiddenNodes = fit.GetHiddenLayer();

        for (int i = 0; i < hiddenNodes.Count; i++)
        {
            hiddenNodeImages[i].color = (hiddenNodes[i].Activated()) ? Color.green : Color.white;

            List<float> weights = hiddenNodes[i].GetWeights();
            for (int x = 0; x < weights.Count; x++)
            {
                hiddenConnectors[i].SetWeight(x, weights[x]);
            }
        }

        List<Neuron> outputNodes = fit.GetOutputNodes();
        for (int i = 0; i < outputNodes.Count; i++)
        {
            outputNodeImages[i].color = (inputNodes[i].Activated()) ? Color.green : Color.white;
            List<float> weights = outputNodes[i].GetWeights();
            for (int x = 0; x < weights.Count; x++)
            {
                outputConnectors[i].SetWeight(x, weights[x]);
            }
        }
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
