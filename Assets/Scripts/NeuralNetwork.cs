using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Try this page for howto
/// The Neural Network that makes up the core of the program
/// 
/// Matthew Demoe
/// Danny Luk
/// Jacky Yang
/// </summary>
public class NeuralNetwork : MonoBehaviour
{
    CarController controller;

    List<Neuron> _inputNodes = new List<Neuron>();
    List<Neuron> _hiddenLayer = new List<Neuron>();
    List<Neuron> _outputNodes = new List<Neuron>();

    /// <summary>
    /// Set up the neural network to be custom tailored to operate a car
    /// </summary>
    void Start()
    {
        controller = GetComponent<CarController>();

        // Add perceptrons to receive inputs
        for (int i = 0; i < 5; i++)
        {
            _inputNodes.Add(new Neuron());
        }

        // Add extra perceptrons to work with our inputs
        for (int i = 0; i < 5; i++)
        {
            _hiddenLayer.Add(new Neuron(_inputNodes));
        }

        // Add output neurons that will send output to the car
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

    /// <summary>
    /// Updates the data in all the neurons in this network
    /// </summary>
    void UpdateNeurons()
    {
        for (int i = 0; i < _inputNodes.Count; i++)
        {
            // Manually set the inputs
            _inputNodes[i].SetInput(controller.GetDistance(i));
        }

        // Take output from the input nodes
        foreach (Neuron n in _hiddenLayer)
        {
            n.CheckParents();
        }

        // Take output from the hidden layer nodes
        foreach (Neuron n in _outputNodes)
        {
            n.CheckParents();
        }
    }

    public List<Neuron> GetInputNodes()
    {
        return _inputNodes;
    }

    public List<Neuron> GetHiddenLayer()
    {
        return _hiddenLayer;
    }

    public List<Neuron> GetOutputNodes()
    {
        return _outputNodes;
    }

    /// <summary>
    /// Manual call to mutate the network's nodes with the nodes of another network
    /// </summary>
    /// <param name="otherNetwork"></param>
    public void MutateNeurons(NeuralNetwork otherNetwork)
    {
        for (int i = 0; i < _inputNodes.Count; i++)
        {
            _inputNodes[i].Mutate(otherNetwork.GetInputNodes()[i]);
        }

        for (int i = 0; i < _hiddenLayer.Count; i++)
        {
            _hiddenLayer[i].Mutate(otherNetwork.GetHiddenLayer()[i]);
        }

        for (int i = 0; i < _outputNodes.Count; i++)
        {
            _outputNodes[i].Mutate(otherNetwork.GetOutputNodes()[i]);
        }
    }
}
