using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inatance;

    public GameObject mainCam;
    public GameObject uiCam;
    public GameObject graph;


    [HideInInspector]
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
            //graph.SetActive(false);
        }

        SetResolution();
    }

    private void Update()
    {
        ClickMachine();
    }

    void OpenGraph()
    {
        uiCam.SetActive(true);
        mainCam.SetActive(false);
        mainCamera = uiCam.GetComponent<Camera>();

        //graph.SetActive(true);
        graph.GetComponent<GraphEditor>().Display();
    }

    public void CloseGraph()
    {
        uiCam.SetActive(false);
        mainCam.SetActive(true);
        mainCamera = mainCam.GetComponent<Camera>();

        //graph.SetActive(false);
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

    /* �ػ� �����ϴ� �Լ� */
    public void SetResolution()
    {
        int setWidth = 1920; // ����� ���� �ʺ�
        int setHeight = 1080; // ����� ���� ����

        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            mainCam.GetComponent<Camera>().rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
            uiCam.GetComponent<Camera>().rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����
            mainCam.GetComponent<Camera>().rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
            uiCam.GetComponent<Camera>().rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
        }
    }
}
