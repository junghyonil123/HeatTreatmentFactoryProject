using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEditor : MonoBehaviour
{
    LineRenderer lineRender;


    public Transform pointAPos;
    public Transform pointBPos;
    public Transform pointCPos;
    public Transform pointDPos;
    
    public Transform coilAPos;
    public Transform coilBPos;
    public Transform coilCPos;
    public Transform coilDPos;

    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        lineRender.startWidth = .05f;
        lineRender.endWidth = .05f;
    }

    public float offset;

    void Update()
    {
        lineRender.SetPosition(0, new Vector3(coilAPos.position.x, coilAPos.position.y, coilAPos.position.z + offset));
        lineRender.SetPosition(1, new Vector3(pointAPos.position.x, pointAPos.position.y, pointAPos.position.z + offset));
    }

    public void DisplayLine()
    {
        //lineRender.SetPosition
    }
}
