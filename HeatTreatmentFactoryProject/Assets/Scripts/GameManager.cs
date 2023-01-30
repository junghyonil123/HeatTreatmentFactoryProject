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

    /* 해상도 설정하는 함수 */
    public void SetResolution()
    {
        int setWidth = 1920; // 사용자 설정 너비
        int setHeight = 1080; // 사용자 설정 높이

        int deviceWidth = Screen.width; // 기기 너비 저장
        int deviceHeight = Screen.height; // 기기 높이 저장

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution 함수 제대로 사용하기

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // 기기의 해상도 비가 더 큰 경우
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // 새로운 너비
            mainCam.GetComponent<Camera>().rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
            uiCam.GetComponent<Camera>().rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // 새로운 Rect 적용
        }
        else // 게임의 해상도 비가 더 큰 경우
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // 새로운 높이
            mainCam.GetComponent<Camera>().rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            uiCam.GetComponent<Camera>().rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
        }
    }
}
