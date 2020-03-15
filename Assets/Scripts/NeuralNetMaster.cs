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

    [SerializeField]
    TrackGenerator trackGenerator;

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
        }
    }


    public void CheckCrashes()
    {
        foreach (GameObject go in cars)
        {
            if (go.GetComponent<CarController>().GetCrashed() == false)
                return;

        }

        FindFittest();

        foreach (GameObject go in cars)
        {
            if (!go.GetComponent<NeuralNetwork>().Equals(_mostFit))
                go.GetComponent<NeuralNetwork>().MutateNeurons(_mostFit.GetComponent<NeuralNetwork>());

            go.transform.SetPositionAndRotation(transform.position, transform.rotation);
            go.GetComponent<CarController>().Init();            
        }

        //Reset
        //trackGenerator.Init();
        
    }

    void FindFittest()
    {
        float highest = -Mathf.Infinity;

        foreach (GameObject go in cars)
        {
            if (go.GetComponent<CarController>().GetFitness() > highest)
            {
                _mostFit = go.GetComponent<NeuralNetwork>();
                highest = go.GetComponent<CarController>().GetFitness();
            }
        }
    }
}
