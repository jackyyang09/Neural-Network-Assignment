using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Neuron
{
    List<Neuron> parents;

    List<float> weights = new List<float>();
    List<float> inputs = new List<float>();
    float _output = 0.0f;

    float _currentActivation = 0.0f;
    float _minThreshold = 0.5f;
    float _maxThreshold = 1.0f;
    float _bias = 0.0f;

    float mutationAmount = 0.15f;

    float _activationThreshold;

    UnityEvent activationFunction = new UnityEvent();

    public Neuron()
    {
        _activationThreshold = Random.Range(_minThreshold, _maxThreshold);
        _bias = Random.Range(0.0f, _minThreshold / 2.0f);
    }

    public Neuron(List<Neuron> p, UnityAction func = null)
    {
        parents = p;

        for (int i = 0; i < parents.Count; i++)
        {
            weights.Add(Random.Range(0.0f, 1.0f));
            inputs.Add(0.0f);
        }

        _activationThreshold = Random.Range(_minThreshold, _maxThreshold);
        _bias = Random.Range(0.0f, _minThreshold);

        if (func != null)
            activationFunction.AddListener(func);
    }

    public void CheckParents()
    {
        for (int i = 0; i < parents.Count; i++)
        {
            inputs[i] = parents[i].GetOutput();

            _currentActivation += inputs[i] * weights[i];
        }

        _currentActivation += _bias;

        if (_currentActivation >= _activationThreshold)
        {
            _output = 1.0f;
            activationFunction.Invoke();
        }

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
        _currentActivation = input;

        if (_currentActivation >= _activationThreshold)
        {
            _output = 1.0f;
        }

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

    public void Mutate(Neuron otherNeuron)
    {
        for (int i = 0; i < weights.Count; i++)
        {
            weights[i] = otherNeuron.GetWeights()[i] * Random.Range(1.0f - mutationAmount, 1.0f+ mutationAmount);
        }

        _bias = otherNeuron.GetBias() * Random.Range(1.0f - mutationAmount, 1.0f + mutationAmount);

        _activationThreshold = otherNeuron.GetActivationThreshold() * Random.Range(1.0f - mutationAmount, 1.0f + mutationAmount);
    }
}
