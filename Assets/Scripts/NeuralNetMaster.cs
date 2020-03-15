using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetMaster : MonoBehaviour
{
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

    NeuralNetwork _mostFit;

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

        InvokeRepeating("SetCameraToMostFit", 0, cameraUpdateFrequency);
    }

    public void CheckCrashes()
    {
        for (int i = 0; i < neuralNets.Count; i++)
        {
            if (!carControllers[i].GetCrashed()) return;
        }

        FindFittest();

        for (int i = 0; i < neuralNets.Count; i++)
        {
            if (!neuralNets[i].Equals(_mostFit))
                neuralNets[i].MutateNeurons(_mostFit);

            neuralNets[i].transform.SetPositionAndRotation(transform.position, transform.rotation);
            carControllers[i].Init();
        }

        //Reset
        //trackGenerator.Init();
        
    }

    void FindFittest()
    {
        float highest = -Mathf.Infinity;

        for (int i = 0; i < carControllers.Count; i++)
        {
            if (carControllers[i].GetFitness() > highest)
            {
                _mostFit = neuralNets[i];
                highest = carControllers[i].GetFitness();
            }
        }
    }

    public void SetCameraToMostFit()
    {
        cam.target = GetFit().transform;
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
            float newHighest = Mathf.Max(c.GetFitness(), highest);
            if (newHighest != highest)
            {
                selected = c;
                highest = newHighest;
            }
        }
        return selected;
    }
}