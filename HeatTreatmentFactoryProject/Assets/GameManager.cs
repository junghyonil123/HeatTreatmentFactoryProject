using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inatance;

    public GameObject mainCam;
    public GameObject UiCam;
    public GameObject graph;

    public Camera mainCamera;

    private void Awake()
    {
        Inatance = this;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (graph.activeSelf)
        {
            graph.SetActive(false);
        }
    }

    private void Update()
    {
        ClickMachine();
    }

    void OpenGraph()
    {
        UiCam.SetActive(true);
        mainCam.SetActive(false);
        mainCamera = UiCam.GetComponent<Camera>();

        graph.SetActive(true);
        graph.GetComponent<GraphEditor>().Display();
    }

    public void CloseGraph()
    {
        UiCam.SetActive(false);
        mainCam.SetActive(true);
        mainCamera = mainCam.GetComponent<Camera>();

        graph.SetActive(false);
    }

    void ClickMachine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("MeltingMachine") && hit.transform.GetComponent<MeltingMachine>().isOperating)
                {
                    OpenGraph();
                }
            }
        }
    }
}
