using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Try this page for howto
/// https://towardsdatascience.com/first-neural-network-for-beginners-explained-with-code-4cfd37e06eaf
/// </summary>
public class NeuralNetwork : MonoBehaviour
{
    //public struct Neuron
    //{
    //    List<Neuron> parents;
    //    List<Neuron> children;
    //    Vector3 weights;
    //    float bias; // Do we even use this lmao
    //}

    CarController controller;

    List<Neuron> _inputNodes = new List<Neuron>();
    List<Neuron> _hiddenLayer = new List<Neuron>();
    List<Neuron> _outputNodes = new List<Neuron>();

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CarController>();

        for (int i = 0; i < 5; i++)
        {
            _inputNodes.Add(new Neuron());
        }

        for (int i = 0; i < 5; i++)
        {
            _hiddenLayer.Add(new Neuron(_inputNodes));
        }

        _outputNodes.Add(new Neuron(_hiddenLayer, controller.Accelerate));
        _outputNodes.Add(new Neuron(_hiddenLayer, controller.Deccelerate));
        _outputNodes.Add(new Neuron(_hiddenLayer, controller.TurnLeft));
        _outputNodes.Add(new Neuron(_hiddenLayer, controller.TurnRight));

    }

    // Update is called once per frame
    void Update()
    {
        UpdateNeurons();
    }

    void UpdateNeurons()
    {
        for (int i = 0; i < _inputNodes.Count; i++)
        {
            _inputNodes[i].SetInput(controller.GetDistance(i));
        }

        foreach (Neuron n in _hiddenLayer)
        {
            n.CheckParents();
        }

        foreach (Neuron n in _outputNodes)
        {
            n.CheckParents();
        }
    }
}
