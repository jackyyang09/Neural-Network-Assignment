using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisualizer : MonoBehaviour
{
    [SerializeField]
    LineRenderer[] connections;

    [SerializeField]
    Vector2 lineWidth;

    Material[] mat;

    private void Awake()
    {
        mat = new Material[connections.Length];
        for (int i = 0; i < connections.Length; i++)
        {
            mat[i] = connections[i].material;
        }
    }

    public void Init()
    {

    }

    public void SetWeight(int x, float weight)
    {
        connections[x].startWidth = Mathf.Lerp(lineWidth.x, lineWidth.y, weight);
        connections[x].endWidth = Mathf.Lerp(lineWidth.x, lineWidth.y, weight);
        mat[x].SetColor("_Color", Color.Lerp(Color.red, Color.green, weight));
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
