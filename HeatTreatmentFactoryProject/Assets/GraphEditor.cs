using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GraphEditor : MonoBehaviour
{

    [Header ("Init")]
    public GraphChart chart;
    public GameObject meltingCore;

    public MeshRenderer coilAMesh;
    public MeshRenderer coilBMesh;
    public MeshRenderer coilCMesh;
    public MeshRenderer coilDMesh;

    public List<float> displayPointList_A = new List<float>();
    public List<float> displayPointList_B = new List<float>();
    public List<float> displayPointList_C = new List<float>();
    public List<float> displayPointList_D = new List<float>();

    public TextMeshProUGUI maxTemperatureText;
    public TextMeshProUGUI minTemperatureText;


    private void Start()
    {
        metingCoreOriginAngle = meltingCore.transform.rotation.eulerAngles;

        Debug.Log(metingCoreOriginAngle);

        for (int i = 0; i < 11; i++)
        {
            displayPointList_A[i]  = Random.Range(0, 500);
            displayPointList_B[i]  = Random.Range(0, 500);
            displayPointList_C[i]  = Random.Range(0, 500);
            displayPointList_D[i]  = Random.Range(0, 500);
        }
    }

    [Header ("Setting")]
    public bool cylinderTurn = false;
    public float changeValueTime = 1;
    public float rotateSpeed;

    public void Display()
    {
        chart.DataSource.StartBatch();

        ResetCore();

        chart.DataSource.ClearCategory("PointA");
        chart.DataSource.ClearCategory("PointB");
        chart.DataSource.ClearCategory("PointC");
        chart.DataSource.ClearCategory("PointD");

        StartCoroutine(DisplayPointAll());
        StartCoroutine(CylenderTurnUseMouse());

        StartCoroutine(CylenderTurn());
        
        chart.DataSource.EndBatch();
    }

    IEnumerator DisplayPointAll()
    {
        for (int i = 0; i < displayPointList_A.Count; i++)
        {
            chart.DataSource.AddPointToCategoryRealtime("PointA", i, displayPointList_A[i], changeValueTime);
            chart.DataSource.AddPointToCategoryRealtime("PointB", i, displayPointList_B[i], changeValueTime);
            chart.DataSource.AddPointToCategoryRealtime("PointC", i, displayPointList_C[i], changeValueTime);
            chart.DataSource.AddPointToCategoryRealtime("PointD", i, displayPointList_D[i], changeValueTime);

            StartCoroutine(ChangeCoilIntencity(i));

            yield return new WaitForSeconds(1f);
        }

        SetValueText();
    }

    IEnumerator CylenderTurn()
    {
        while (true)
        {
            yield return new WaitUntil(() => !Input.GetMouseButton(0));
            yield return new WaitUntil(() => cylinderTurn);

            //cylender.transform.Rotate(Vector3.up * Time.deltaTime * 20, Space.World);
            meltingCore.transform.Rotate(new Vector3(0f, 1f, 0f));
            yield return null;
        }
    }

    Ray ray;
    RaycastHit hit;

    Vector2 lastMousePos = Vector2.zero;
    Vector2 curMousePos = Vector2.zero;
    Vector2 moveVec;

    bool isClickCore = false;
    //bool isFirstClick = false;

    IEnumerator CylenderTurnUseMouse()
    {
        while (true)
        {
            //Debug.Log(isFirstClick);

            if (Input.GetMouseButtonUp(0))
            {
                //�ʱ�ȭ
                isClickCore = false;
              //  isFirstClick = true;
            }

            yield return new WaitUntil(() => Input.GetMouseButton(0));

            ray = GameManager.Inatance.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("MeltingCore"))
                {
                    isClickCore = true;
                }
            }

            if (isClickCore)
            {

                curMousePos = Input.mousePosition;

                if (Input.GetMouseButtonDown(0))
                {
                    lastMousePos = curMousePos;
                    //isFirstClick = false;
                }

                moveVec = curMousePos - lastMousePos;



                meltingCore.transform.Rotate(new Vector3(moveVec.y, moveVec.x, moveVec.y) * rotateSpeed * Time.deltaTime * -1f);

                lastMousePos = curMousePos;
            }

            yield return null;
        }

    }


    public Vector3 metingCoreOriginAngle;
    public void ResetCore()
    {
        //meltingCore.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        Debug.Log("�����");
        meltingCore.transform.rotation = Quaternion.Euler(metingCoreOriginAngle);
        minTemperatureText.text = "";
        maxTemperatureText.text = "";
    }

    public void AutoTurn()
    {
        cylinderTurn = !cylinderTurn;
    }


    private Color lastCoilAEmission = Color.red;
    private Color lastCoilBEmission = Color.red;
    private Color lastCoilCEmission = Color.red;
    private Color lastCoilDEmission = Color.red;

    public IEnumerator ChangeCoilIntencity(int number)
    {
        WaitForSeconds waitTime = new WaitForSeconds(changeValueTime * 0.01f);

        Color coilAEmission;
        Color coilBEmission;
        Color coilCEmission;
        Color coilDEmission;

        for (int i = 0; i < 100; i++)
        {
            coilAEmission = Color.red * displayPointList_A[number];
            coilBEmission = Color.red * displayPointList_B[number];
            coilCEmission = Color.red * displayPointList_C[number];
            coilDEmission = Color.red * displayPointList_D[number];

            coilAMesh.material.SetColor("_EmissiveColor", Color.red * lastCoilAEmission - (lastCoilAEmission - coilAEmission) * i * 0.01f);
            coilBMesh.material.SetColor("_EmissiveColor", Color.red * lastCoilBEmission - (lastCoilBEmission - coilBEmission) * i * 0.01f);
            coilCMesh.material.SetColor("_EmissiveColor", Color.red * lastCoilCEmission - (lastCoilCEmission - coilCEmission) * i * 0.01f);
            coilDMesh.material.SetColor("_EmissiveColor", Color.red * lastCoilDEmission - (lastCoilDEmission - coilDEmission) * i * 0.01f);

            lastCoilAEmission = coilAEmission;
            lastCoilBEmission = coilBEmission;
            lastCoilCEmission = coilCEmission;
            lastCoilDEmission = coilDEmission;

            yield return waitTime;
        }
    }

    float maxValue;
    float minValue;

    public void SetValueText()
    {
        maxValue = 0;
        minValue = 9999;

        for (int i = 0; i < 11; i++)
        {
            if (displayPointList_A[i] > maxValue)
            {
                maxValue = displayPointList_A[i];
            }
            if (displayPointList_A[i] < minValue)
            {
                minValue = displayPointList_A[i];
            }
            if (displayPointList_B[i] > maxValue)
            {
                maxValue = displayPointList_B[i];
            }
            if (displayPointList_B[i] < minValue)
            {
                minValue = displayPointList_B[i];
            }
            if (displayPointList_C[i] > maxValue)
            {
                maxValue = displayPointList_C[i];
            }
            if (displayPointList_C[i] < minValue)
            {
                minValue = displayPointList_C[i];
            }
            if (displayPointList_D[i] > maxValue)
            {
                maxValue = displayPointList_D[i];
            }
            if (displayPointList_D[i] < minValue)
            {
                minValue = displayPointList_D[i];
            }
        }

        maxTemperatureText.text = maxValue + "��C";
        minTemperatureText.text = minValue + "��C";
    }

    // Update is called once per frame
}
