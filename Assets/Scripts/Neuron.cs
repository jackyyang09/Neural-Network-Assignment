using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that handles the logic for all individual neurons that compose our neural network
/// 
/// Danny Luk
/// Matthew Demoe
/// Jacky Yang
/// </summary>
public class Neuron
{
    /// <summary>
    /// A reference to all the neurons that come before this one
    /// </summary>
    List<Neuron> parents;

    //////////////////////////////
    // Variables relevant to 
    List<float> weights = new List<float>();
    List<float> inputs = new List<float>();
    float _output = 0.0f;

    float _currentActivation = 0.0f;
    float _minThreshold = 0.5f;
    float _maxThreshold = 1.0f;
    float _bias = 0.0f;
    //////////////////////////////

    /// <summary>
    /// A constant variable that defines amount to mutate neuron values by
    /// </summary>
    float mutationAmount = 0.1f;

    /// <summary>
    /// Constantly shifting threshold that defines when the neuron outputs 1
    /// </summary>
    float _activationThreshold;

    /// <summary>
    /// Constant value that defines the probability that the neuron's mutation generates
    /// completely new values for its parameters instead
    /// </summary>
    float randomProbability = 0.2f;

    /// <summary>
    /// Car controller function to call if the neuron outputs 1,
    /// only applicable to output neurons
    /// </summary>
    UnityEvent activationFunction = new UnityEvent();

    public Neuron()
    {
        // Assign a random activation within a range threshold on creation
        _activationThreshold = Random.Range(_minThreshold, _maxThreshold);
        // Assign a random bias value within a range on creation
        _bias = Random.Range(0.0f, _minThreshold / 2.0f);
    }

    public Neuron(List<Neuron> p, UnityAction func = null)
    {
        parents = p;

        //Create a weight for each parent as well as a holder for the output of each parent
        for (int i = 0; i < parents.Count; i++)
        {
            weights.Add(Random.Range(0.0f, 1.0f));
            inputs.Add(0.0f);
        }

        // Assign a random activation within a range threshold on creation
        _activationThreshold = Random.Range(_minThreshold, _maxThreshold);
        // Assign a random bias value within a range on creation
        _bias = Random.Range(0.0f, _minThreshold / 2.0f);

        //Function that will be called when the neuron fires
        if (func != null)
            activationFunction.AddListener(func);
    }

    /// <summary>
    /// Perform calculations on the individual neuron
    /// </summary>
    public void CheckParents()
    {
        // Receive input from our parent nodes
        for (int i = 0; i < parents.Count; i++)
        {
            inputs[i] = parents[i].GetOutput();

            // Apply weights to the inputs
            _currentActivation += inputs[i] * weights[i]; 
        }

        // Apply our bias level, as if we're using one
        _currentActivation += _bias;

        // Is this value significant enough?
        if (_currentActivation >= _activationThreshold)
        {
            // Round our input to 1 cause no value this high matters
            _output = 1.0f;
            // Invokes the neuron's designated car-related function
            activationFunction.Invoke();
        }
        // It's a dud, register it as 0
        else
            _output = 0.0f;

        _currentActivation = 0.0f;
    }

    public float GetOutput()
    {
        return _output;
    }

    //Used for input nodes, because they have no parents, and are just taking world input
    public void SetInput(float input)
    {
        //Our input is just the input
        _currentActivation = input;

        // Is this value significant enough?
        if (_currentActivation >= _activationThreshold)
        {
            _output = 1.0f;
        }
        // It's a dud, register it as 0
        else
            _output = 0.0f;

        _currentActivation = 0.0f;
    }

    public List<float> GetWeights()
    {
        return weights;
    }

    public float GetBias()
    {
        return _bias;
    }

    public float GetActivationThreshold()
    {
        return _activationThreshold;
    }

    public bool Activated()
    {
        return _output == 1;
    }

    /// <summary>
    /// Mutates the current neuron's weights and other parameters based on another neuron,
    /// usually the one with the highest fitness
    /// </summary>
    /// <param name="otherNeuron"></param>
    public void Mutate(Neuron otherNeuron)
    {
        float prob;

        //For each weight, as well as bias and activation threshold
        //There is a probability that we will either create a new random value
        //Otherwise we will copy the value from the network we are mutating from & apply a slight mutation
        for (int i = 0; i < weights.Count; i++)
        {
            prob = Random.Range(0.0f, 1.0f);

            if (prob > randomProbability)
                weights[i] = Random.Range(0.0f, 1.0f);

            // Increase/Decrease weights by a random modifier of size mutationAmount
            else
                weights[i] = otherNeuron.GetWeights()[i] * Random.Range(1 - mutationAmount, 1 + mutationAmount);
        }

        prob = Random.Range(0.0f, 1.0f);

        if (prob > randomProbability)
            _bias = Random.Range(0.0f, _minThreshold / 2.0f);

        // Increase/Decrease bias by a random modifier of size mutationAmount
        else
            _bias = otherNeuron.GetBias() * Random.Range(1 - mutationAmount, 1 + mutationAmount);

        prob = Random.Range(0.0f, 1.0f);

        if (prob > randomProbability)
            _activationThreshold = Random.Range(_minThreshold, _maxThreshold);

        // Increase/Decrease activation threshold by a random modifier of size mutationAmount
        else
            _activationThreshold = otherNeuron.GetActivationThreshold() * Random.Range(1 - mutationAmount, 1 + mutationAmount);
    }
}
