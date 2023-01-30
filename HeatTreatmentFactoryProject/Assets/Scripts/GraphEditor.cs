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

    public List<GameObject> cursorList = new List<GameObject>();

    private void Start()
    {
        metingCoreOriginAngle = meltingCore.transform.rotation.eulerAngles;

        Debug.Log(metingCoreOriginAngle);

        for (int i = 0; i < 720; i++)
        {
            displayPointList_A.Add(0);
            displayPointList_B.Add(0);
            displayPointList_C.Add(0);
            displayPointList_D.Add(0);
        }

        for (int i = 0; i < 720; i++)
        {
            displayPointList_A[i]  = Random.Range(0, 1000);
            displayPointList_B[i]  = Random.Range(0, 1000);
            displayPointList_C[i]  = Random.Range(0, 1000);
            displayPointList_D[i]  = Random.Range(0, 1000);
        }

        CandleChartData.SetHorizontalViewSize(displayPointList_A.Count * 0.00277778);
    }

    [Header ("Setting")]
    public bool cylinderTurn = false;
    public float rotateSpeed;

    private float changeValueTime = 0.5f;
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
        for (int i = 0; i < 21; i++)
        {
            int point;

            if (i == 20)
            {
                point = displayPointList_A.Count - 1;
            }
            else
            {
                point = (int)displayPointList_A.Count / 20 * i;
            }

            chart.DataSource.AddPointToCategoryRealtime("PointA", point * 0.00277778, displayPointList_A[point], changeValueTime * 2);
            chart.DataSource.AddPointToCategoryRealtime("PointB", point * 0.00277778, displayPointList_B[point], changeValueTime * 2);
            chart.DataSource.AddPointToCategoryRealtime("PointC", point * 0.00277778, displayPointList_C[point], changeValueTime * 2);
            chart.DataSource.AddPointToCategoryRealtime("PointD", point * 0.00277778, displayPointList_D[point], changeValueTime * 2);

            Debug.Log("코루틴 실행");
            StartCoroutine(ChangeCoilIntencity(i));

            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => !isOnChangeCoilIntencity);
        }

        SetValueText();
    }

    IEnumerator CylenderTurn()
    {
        while (true)
        {
            yield return new WaitUntil(() => !Input.GetMouseButton(0));
            yield return new WaitUntil(() => cylinderTurn);

            DeletePointAnim();
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
                //초기화
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
                DeletePointAnim();

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
        cylinderTurn = false;
        meltingCore.transform.rotation = Quaternion.Euler(metingCoreOriginAngle);
        minTemperatureText.text = "";
        maxTemperatureText.text = "";

        StartPointAnim();
    }

    public void AutoTurn()
    {
        cylinderTurn = !cylinderTurn;
    }

    private Color lastCoilAEmission = Color.red;
    private Color lastCoilBEmission = Color.red;
    private Color lastCoilCEmission = Color.red;
    private Color lastCoilDEmission = Color.red;

    bool isOnChangeCoilIntencity = false;
    public IEnumerator ChangeCoilIntencity(int number)
    {
        WaitForSeconds waitTime = new WaitForSeconds(changeValueTime * 0.01f);

        isOnChangeCoilIntencity = true;

        Debug.Log("코루틴 시작됨");

        Color coilAEmission;
        Color coilBEmission;
        Color coilCEmission;
        Color coilDEmission;

        coilAMesh.enabled = true;
        coilBMesh.enabled = true;
        coilCMesh.enabled = true;
        coilDMesh.enabled = true;

        coilAEmission = Color.red * (displayPointList_A[number] * 0.2f);
        coilBEmission = Color.red * (displayPointList_B[number] * 0.2f);
        coilCEmission = Color.red * (displayPointList_C[number] * 0.2f);
        coilDEmission = Color.red * (displayPointList_D[number] * 0.2f);

        for (int i = 0; i < 100; i++)
        {
            coilAMesh.sharedMaterial.SetColor("_EmissiveColor", lastCoilAEmission - (lastCoilAEmission - coilAEmission) * i * 0.01f);
            Color a = (lastCoilAEmission - coilAEmission) * i * 0.01f;
            Debug.Log(i + "번째" + coilAMesh.sharedMaterial.GetColor("_EmissiveColor"));
            Debug.Log(lastCoilAEmission);
            Debug.Log(coilAEmission);
            //Debug.Log("LastCoilAEmission" + lastCoilAEmission);
            //Debug.Log("CoilAEmission" + coilAEmission);
            coilBMesh.sharedMaterial.SetColor("_EmissiveColor", lastCoilBEmission - (lastCoilBEmission - coilBEmission) * i * 0.01f);
            coilCMesh.sharedMaterial.SetColor("_EmissiveColor", lastCoilCEmission - (lastCoilCEmission - coilCEmission) * i * 0.01f);
            coilDMesh.sharedMaterial.SetColor("_EmissiveColor", lastCoilDEmission - (lastCoilDEmission - coilDEmission) * i * 0.01f);

            yield return waitTime;
        }

        lastCoilAEmission = coilAEmission;
        lastCoilBEmission = coilBEmission;
        lastCoilCEmission = coilCEmission;
        lastCoilDEmission = coilDEmission;

        isOnChangeCoilIntencity = false;
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

        maxTemperatureText.text = maxValue + "°C";
        minTemperatureText.text = minValue + "°C";
    }

    public void StartPointAnim()
    {
        for (int i = 0; i < cursorList.Count; i++)
        {
            cursorList[i].SetActive(true);
            cursorList[i].GetComponent<Animator>().SetTrigger("Reset");
        }
    }

    public void DeletePointAnim()
    {
        if (!cursorList[0].activeSelf)
        {
            return;
        }

        for (int i = 0; i < cursorList.Count; i++)
        {
            cursorList[i].SetActive(false);
        }
    }

    // Update is called once per frame
}
