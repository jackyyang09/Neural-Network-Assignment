using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A master class that manages the Neural Networks of all the cars in the scene
/// 
/// Matthew Demoe
/// Danny Luk
/// Jacky Yang
/// </summary>
public class NeuralNetMaster : MonoBehaviour
{
    float timer = 0;
    /// <summary>
    /// Neural network regenerates automatically after this many seconds passes
    /// </summary>
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

    /// <summary>
    /// Singleton logic
    /// </summary>
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
        // Switch between targets using left and right
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

        // Create a new set of cars if the simulation goes on for too long
        if (timer >= generationTimer)
            NewGeneration();
    }    

    /// <summary>
    /// Checks if all the cars crashed or not
    /// </summary>
    public void CheckCrashes()
    {
        for (int i = 0; i < neuralNets.Count; i++)
        {
            // Cancel out if at least one is alive
            if (!carControllers[i].GetCrashed()) return;
        }

        // Create a new generation of cars if they've all "crashed"
        NewGeneration();
    }

    /// <summary>
    /// Create a new generation of cars and mutate the less successful ones
    /// </summary>
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

    /// <summary>
    /// A helper method that works with the UI to focus on the car with the highest fitness
    /// Can be disabled
    /// </summary>
    /// <param name="b"></param>
    public void ToggleHyperCam(bool b)
    {
        if (b)
        {
            targetSearchRoutine = StartCoroutine(SetCameraToMostFit());
        } 
        else
        {
            StopCoroutine(targetSearchRoutine);
            targetSearchRoutine = null;
        }
    }

    public NeuralNetwork GetSelectedCar()
    {
        return neuralNets[selectedCar];
    }

    public CarController GetSelectedCarController()
    {
        return carControllers[selectedCar];
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
        // Iterates through the array of cars and searches for the fittest
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
        // Designate this car as the "selectedCar" for the UI to reference
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
        // Designate this car as the "selectedCar" for the UI to reference
        selectedCar = carControllers.IndexOf(selected);
        return selected.GetComponent<NeuralNetwork>();
    }
}