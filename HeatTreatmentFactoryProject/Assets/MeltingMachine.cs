using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltingMachine : MonoBehaviour
{
    public bool isOperating;

    public MeshRenderer lightBlock;

    public Material operationMaterial;
    public Material unOperationMaterial;

    private void Start()
    {
        SetDefaultMesh();
        SetLightBlockColor();
    }

    public void SetDefaultMesh()
    {
        MeshRenderer[] meshRendererArray = transform.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshRendererArray.Length; i++)
        {
            meshRendererArray[i].enabled = true;
        }
    }

    public void SetLightBlockColor()
    {
        if (isOperating)
        {
            lightBlock.sharedMaterial = operationMaterial;
        }
        else
        {
            lightBlock.sharedMaterial = unOperationMaterial;
        }
    }

}
