using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NeuralNetMaster : MonoBehaviour
{
    [SerializeField]
    float timescale = 1.0f;

    float timer = 0;
    [SerializeField]
    float generationTimer = 10.0f;

    [SerializeField]
    GameObject carParent;

    [SerializeField]
    List<GameObject> cars = new List<GameObject>();

    List<NeuralNetwork> neuralNets = new List<NeuralNetwork>();
    List<CarController> carControllers = new List<CarController>();

    [SerializeField]
    TrackGenerator trackGenerator;

    [SerializeField]
    FollowCamera cam;
    [SerializeField]
    float cameraUpdateFrequency;

    int selectedCar;

    List<NeuralNetwork> sortedNets = new List<NeuralNetwork>();

    NeuralNetwork _mostFit;

    public delegate void OnReset();

    public event OnReset onReset;

    Coroutine targetSearchRoutine;

    private static NeuralNetMaster _instance;

    public static NeuralNetMaster Instance
    {
        get
        {
            _instance = FindObjectOfType<NeuralNetMaster>();

            if (_instance == null)
                _instance = new GameObject().AddComponent<NeuralNetMaster>();

            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < carParent.transform.childCount; i++)
        {
            cars.Add(carParent.transform.GetChild(i).gameObject);
            neuralNets.Add(carParent.transform.GetChild(i).GetComponent<NeuralNetwork>());
            carControllers.Add(carParent.transform.GetChild(i).GetComponent<CarController>());
        }
    }

    private void Update()
    {

        Time.timeScale = timescale;

        if (targetSearchRoutine == null)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedCar = (int)Mathf.Repeat(selectedCar - 1, neuralNets.Count);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedCar = (selectedCar + 1) % neuralNets.Count;
            }
            cam.target = carControllers[selectedCar].transform;
            timer += Time.deltaTime;
        }
        if (timer >= generationTimer)
            NewGeneration();
    }    
    public void CheckCrashes()
    {
        for (int i = 0; i < neuralNets.Count; i++)
        {
            if (!carControllers[i].GetCrashed()) return;
        }

        NewGeneration();
    }

    void NewGeneration()
    {
        timer = 0;

        //Sort Neural Networks by their fitness
        neuralNets = neuralNets.OrderByDescending(n => n.GetComponent<CarController>().GetFitness()).ToList();

        //Mutate lower half of (sorted) networks
        for (int i = 0; i < neuralNets.Count; i++)
        {
            if(i >= (neuralNets.Count / 2))
                neuralNets[i].MutateNeurons(neuralNets[i - (neuralNets.Count / 2)]);

            //Reset each car
            carControllers[i].Init();
        }

        //Reset
        onReset?.Invoke(); //Invoke event
    }

    public void ToggleHyperCam(bool b)
    {
        if (b)
        {
            targetSearchRoutine = StartCoroutine(SetCameraToMostFit());
        } 
        else
        {
            StopCoroutine(SetCameraToMostFit());
            targetSearchRoutine = null;
        }
    }

    public NeuralNetwork GetSelectedCar()
    {
        return neuralNets[selectedCar];
    }

    IEnumerator SetCameraToMostFit()
    {
        while (true)
        {
            cam.target = GetFit().transform;
            yield return new WaitForSecondsRealtime(cameraUpdateFrequency);
        }
    }

    /// <summary>
    /// Returns the most fit car
    /// </summary>
    /// <returns></returns>
    public CarController GetFit()
    {
        float highest = -1;
        CarController selected = carControllers[0]; // Default
        foreach (CarController c in carControllers)
        {
            if (c.GetCrashed()) continue;
            float newHighest = Mathf.Max(c.GetFitness(), highest);
            if (newHighest != highest)
            {
                selected = c;
                highest = newHighest;
            }
        }
        selectedCar = carControllers.IndexOf(selected);
        return selected;
    }

    /// <summary>
    /// Returns the most fit car
    /// </summary>
    /// <returns></returns>
    public NeuralNetwork GetFittestNetwork()
    {
        float highest = -1;
        CarController selected = carControllers[0]; // Default
        foreach (CarController c in carControllers)
        {
            if (c.GetCrashed()) continue;
            float newHighest = Mathf.Max(c.GetFitness(), highest);
            if (newHighest != highest)
            {
                selected = c;
                highest = newHighest;
            }
        }
        selectedCar = carControllers.IndexOf(selected);
        return selected.GetComponent<NeuralNetwork>();
    }
}