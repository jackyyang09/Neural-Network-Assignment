using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NeuralNetMaster : MonoBehaviour
{
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
        FindFittest();

        for (int i = 0; i < sortedNets.Count; i++)
        {
            if(i >= (sortedNets.Count / 2))
                sortedNets[i].MutateNeurons(sortedNets[0]);

            //sortedNets[i].transform.SetPositionAndRotation(transform.position, transform.rotation);
            carControllers[i].Init();
        }

        //Reset
        //trackGenerator.Init();
        onReset?.Invoke(); //Invoke event
    }

    void FindFittest()
    {
        //sortedNets.Clear();
        //
        //while (sortedNets.Count < neuralNets.Count)
        //{
        //    float highest = -Mathf.Infinity;
        //    int highestIndex = 0;
        //
        //    for (int i = 0; i < carControllers.Count; i++)
        //    {
        //        if (carControllers[i].GetFitness() > highest)
        //        {
        //            if (!sortedNets.Contains(neuralNets[i]))
        //            {
        //                highestIndex = i;
        //                highest = carControllers[i].GetFitness();
        //            }
        //        }
        //    }
        //
        //    sortedNets.Add(neuralNets[highestIndex]);
        //}   

        sortedNets = neuralNets.OrderByDescending(n => n.GetComponent<CarController>().GetFitness()).ToList();

        //neuralNets = neuralNets.Sort((n1, n2) => n1.gameObject.GetComponent<CarController>().GetFitness().CompareTo(n2.gameObject.GetComponent<CarController>().GetFitness()));

    }

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