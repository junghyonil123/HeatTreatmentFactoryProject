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

        //Debug.Log(metingCoreOriginAngle);

        for (int i = 0; i < graphNum; i++)
        {
            displayPointList_A.Add(0);
            displayPointList_B.Add(0);
            displayPointList_C.Add(0);
            displayPointList_D.Add(0);
        }

        for (int i = 1; i < graphNum; i++)
        {

            if (i <= graphNum / 2)
            {
                displayPointList_A[i] = Mathf.Clamp(displayPointList_A[i-1] + Random.Range(0, 2), 0, 999);
                displayPointList_C[i] = Mathf.Clamp(displayPointList_C[i-1] + Random.Range(0, 2), 0, 999);
                displayPointList_D[i] = Mathf.Clamp(displayPointList_D[i-1] + Random.Range(0, 2), 0, 999);
                displayPointList_B[i] = Mathf.Clamp(displayPointList_B[i-1] + Random.Range(0, 2), 0, 999);
            }
            else
            {
                displayPointList_A[i] = Mathf.Clamp(displayPointList_A[i - 1] + Random.Range(-1, 1), 0, 999);
                displayPointList_C[i] = Mathf.Clamp(displayPointList_C[i - 1] + Random.Range(-1, 1), 0, 999);
                displayPointList_D[i] = Mathf.Clamp(displayPointList_D[i - 1] + Random.Range(-1, 1), 0, 999);
                displayPointList_B[i] = Mathf.Clamp(displayPointList_B[i - 1] + Random.Range(-1, 1), 0, 999);
            }
            
        }

        SetDefaultCoreMesh();

        //CandleChartData.SetHorizontalViewSize(displayPointList_A.Count * 0.00277778);


        StartCoroutine(CylenderTurnUseMouse());
        StartCoroutine(CylenderTurn());
    }

    [Header ("Setting")]
    public bool cylinderTurn = false;
    public float rotateSpeed;
    public float graphNum;
    private float changeValueTime = 0.5f;

    public void Display()
    {
        if (isOnDisplayPoint)
        {
            return;
        }

        chart.DataSource.StartBatch();

        ResetCore();

        chart.DataSource.ClearCategory("PointA");
        chart.DataSource.ClearCategory("PointB");
        chart.DataSource.ClearCategory("PointC");
        chart.DataSource.ClearCategory("PointD");

        Debug.Log("실행");

        StartCoroutine(DisplayPointAll());
        
        chart.DataSource.EndBatch();
    }

    public static bool isOnDisplayPoint = false;


    public void SetDefaultValue()
    {
        Debug.Log(iCount);

        if (iCount < 0)
        {
            return;
        }

        for (int i = 0; i < iCount; i++)
        {
            chart.DataSource.AddPointToCategory("PointA", i, displayPointList_A[i]);
            chart.DataSource.AddPointToCategory("PointB", i, displayPointList_B[i]);
            chart.DataSource.AddPointToCategory("PointC", i, displayPointList_C[i]);
            chart.DataSource.AddPointToCategory("PointD", i, displayPointList_D[i]);
        }
    }

    public int iCount = 0;
    IEnumerator DisplayPointAll()
    {
        isOnDisplayPoint = true;

        Debug.Log("리셋");
        maxValue = 0;
        minValue = 9999;

        for (int i = 0; i < 2001; i++)
        {
            int point;

            if (i == 2001)
            {
                point = displayPointList_A.Count - 1;
            }
            else
            {
                point = (int)(displayPointList_A.Count / 2000 * i);
            }

            chart.DataSource.AddPointToCategoryRealtime("PointA", point, displayPointList_A[point], changeValueTime * 2);
            chart.DataSource.AddPointToCategoryRealtime("PointB", point, displayPointList_B[point], changeValueTime * 2);
            chart.DataSource.AddPointToCategoryRealtime("PointC", point, displayPointList_C[point], changeValueTime * 2);
            chart.DataSource.AddPointToCategoryRealtime("PointD", point, displayPointList_D[point], changeValueTime * 2);

            StartCoroutine(ChangeCoilIntencity(i));

            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => !isOnChangeCoilIntencity);
            yield return new WaitUntil(() => GameManager.Insatance.isGraphOn);

            if (breakDisplayPointCoroutine)
            {
                breakDisplayPointCoroutine = false;
                isOnDisplayPoint = false;
                yield break;
            }

            SetValueText(point);

            isOnDisplayPoint = false;
        }

        //for (iCount = 0; iCount < displayPointList_A.Count; iCount++)
        //{

        //    chart.DataSource.AddPointToCategoryRealtime("PointA", iCount, displayPointList_A[iCount], changeValueTime * 2);
        //    chart.DataSource.AddPointToCategoryRealtime("PointB", iCount, displayPointList_B[iCount], changeValueTime * 2);
        //    chart.DataSource.AddPointToCategoryRealtime("PointC", iCount, displayPointList_C[iCount], changeValueTime * 2);
        //    chart.DataSource.AddPointToCategoryRealtime("PointD", iCount, displayPointList_D[iCount], changeValueTime * 2);

        //    StartCoroutine(ChangeCoilIntencity(iCount));

        //    yield return new WaitForSeconds(1f);
        //    yield return new WaitUntil(() => !isOnChangeCoilIntencity);
        //    yield return new WaitUntil(() => GameManager.Insatance.isGraphOn);
            
        //    SetValueText(iCount);
        //}

        isOnDisplayPoint = false;
    }

    public bool breakDisplayPointCoroutine = false;
    public void DisplayPointImmediate()
    {
        breakDisplayPointCoroutine = true;

        for (int i = 0; i < 2001; i++)
        {
            int point;

            if (i == 2000)
            {
                point = displayPointList_A.Count - 1;
            }
            else
            {
                point = (int)(displayPointList_A.Count / 2000 * i);
            }

            chart.DataSource.AddPointToCategory("PointA", point, displayPointList_A[point]);
            chart.DataSource.AddPointToCategory("PointB", point, displayPointList_B[point]);
            chart.DataSource.AddPointToCategory("PointC", point, displayPointList_C[point]);
            chart.DataSource.AddPointToCategory("PointD", point, displayPointList_D[point]);

            SetValueText(i);
        }
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
    bool isTopSide = false;
    bool isBottomSide = false;

    IEnumerator CylenderTurnUseMouse()
    {
        while (true)
        {
            //Debug.Log(isFirstClick);

            if (Input.GetMouseButtonUp(0))
            {
                //초기화
                isClickCore = false;
                isTopSide = false;
                isBottomSide = false;
                //  isFirstClick = true;
            }

            yield return new WaitUntil(() => Input.GetMouseButton(0));

            ray = GameManager.Insatance.mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("MeltingCore") || hit.transform.CompareTag("MeltingCoreTop") || hit.transform.CompareTag("MeltingCoreBottom"))
                {
                    if (hit.transform.CompareTag("MeltingCoreTop") && !isClickCore)
                    {
                        isTopSide = true;
                    }
                    else if (hit.transform.CompareTag("MeltingCoreBottom") && !isClickCore)
                    {
                        isBottomSide = true;
                    }

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

                moveVec *= rotateSpeed * Time.deltaTime;

                if (isTopSide)
                {
                    meltingCore.transform.Rotate(0f, 0f, moveVec.x * -1f, Space.World);
                }
                else if (isBottomSide)
                {
                    meltingCore.transform.Rotate(0f, 0f, moveVec.x, Space.World);
                }
                else
                {
                    meltingCore.transform.Rotate(0f, moveVec.x * -1, 0f, Space.World);
                    meltingCore.transform.Rotate(moveVec.y, 0f, 0f, Space.World);
                }

                lastMousePos = curMousePos;
            }

            yield return null;
        }

    }

    public void SetDefaultCoreMesh()
    {
        var mesh = meltingCore.GetComponentsInChildren<MeshRenderer>();

        foreach (var item in mesh)
        {
            item.enabled = true;
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

    public void SetValueText(int num)
    {
        if (displayPointList_A[num] > maxValue)
        {
            maxValue = displayPointList_A[num];
        }
        if (displayPointList_A[num] < minValue)
        {
            minValue = displayPointList_A[num];
        }
        if (displayPointList_B[num] > maxValue)
        {
            maxValue = displayPointList_B[num];
        }
        if (displayPointList_B[num] < minValue)
        {
            minValue = displayPointList_B[num];
        }
        if (displayPointList_C[num] > maxValue)
        {
            maxValue = displayPointList_C[num];
        }
        if (displayPointList_C[num] < minValue)
        {
            minValue = displayPointList_C[num];
        }
        if (displayPointList_D[num] > maxValue)
        {
            maxValue = displayPointList_D[num];
        }
        if (displayPointList_D[num] < minValue)
        {
            minValue = displayPointList_D[num];
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
