using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetMaster : MonoBehaviour
{
    [SerializeField]
    List<GameObject> cars = new List<GameObject>();

    List<NeuralNetwork> neuralNets = new List<NeuralNetwork>();

    [SerializeField]
    TrackGenerator trackGenerator;

    private static NeuralNetMaster _instance;

    public static NeuralNetMaster Instance
    {
        get
        {
            if (_instance == null)
                _instance = new NeuralNetMaster();

            return _instance;
        }
    }

    public NeuralNetMaster() { }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

        foreach (GameObject go in cars)
        {
            neuralNets.Add(go.GetComponent<NeuralNetwork>());
        }
    }


    public void CheckCrashes()
    {
        foreach (GameObject go in cars)
        {
            if (go.GetComponent<CarController>().GetCrashed() == false)
                return;

        }

        foreach (GameObject go in cars)
        {
            go.GetComponent<NeuralNetwork>().MutateNeurons(go.GetComponent<NeuralNetwork>());

            go.transform.SetPositionAndRotation(transform.position, transform.rotation);
            go.GetComponent<CarController>().Init();
        }

        //Reset
        trackGenerator.Init();
        
    }
}
