using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewBehaviourScript : MonoBehaviour {

    const int BET_NUM_CNT = 8;
    const int BET_CNT = 25;

    bool isTouched;
    bool betPadDragable;
    bool isAutoMoving;
    Vector2 finalDragDelta;
    List<int> BetNumArrayList;

    public UILabel[] GO_BetNumLabel;

    public GameObject GO_BET_PAD_NUM;

    readonly int[] betNumArray = 
    {
        5,
        10,
        15,
        20,
        25,
        30,
        35,
        40,
        45,
        50,
        55 ,
        60,
        65,
        70,
        75,
        80,
        85 ,
        90,
        95,
        100 ,
        105,
        110,
        115,
        120,
        125
    };

    int m_idx_focus = 0;
    string m_text_focus = "";

    string m_debug = "";

    void OnGUI()
    {
        GUILayout.Label("m_idx_focus " + m_idx_focus + " , m_text_focus " + m_text_focus);
        GUILayout.Label("m_debug is " + m_debug);
    }

    // Use this for initialization
    void Start () {

        UIEventListener.Get(gameObject).onDragStart = onDragStart;
        UIEventListener.Get(gameObject).onDragEnd = onDragEnd;
        UIEventListener.Get(gameObject).onDragOut = onDragOut;
        UIEventListener.Get(gameObject).onDragOver = onDragOver;
        UIEventListener.Get(gameObject).onDrag = onDrag;

        BetNumArrayList = new List<int>();

        for (int i = 0; i < betNumArray.Length; i++)
        {
            BetNumArrayList.Add(betNumArray[i]);
        }
    }

    void onDragStart(GameObject go)
    {
        //print("onDragStart");

        isTouched = true;
        betPadDragable = true;
        //resetAutoHideSec();
    }
    void onDragEnd(GameObject go)
    {
        //print("onDragEnd");

        betPadDragable = false;
        isTouched = false;

        if (isAutoMoving)
        {
            return;
        }

        //Debug.Log("DragEnd");
        //Debug.Log(finalDragDelta.magnitude);

        continueRotate();
    }
    void onDragOut(GameObject go)
    {
        //print("onDragOut");
        betPadDragable = false;

    }
    void onDragOver(GameObject go)
    {
        //print("onDragOver");
        betPadDragable = true;

    }
    void onDrag(GameObject go ,Vector2 delta)
    {
        //print("onDrag");

        delta = delta / Screen.width;
        finalDragDelta = delta;

        float length = delta.magnitude * 1000;
        //Debug.Log(length);
        if (delta.y > 0)
        {
            addBetPadRotationZ(length * (-1));
        }
        else if (delta.y < 0)
        {
            addBetPadRotationZ(length);
        }
    }
    private void addBetPadRotationZ(float value)
    {
        value *= 0.3f;

        float ori_z = GO_BET_PAD_NUM.transform.eulerAngles.z;

        GO_BET_PAD_NUM.transform.eulerAngles = new Vector3(0, 0, ori_z + value);

        UpdateBetLabel();
    }

    // 更新 UILabel 顯示的下注值
    private void UpdateBetLabel()
    {
        int focus_Index_Label = getBetNumFocusIndex();
        int focus_Value_Label = int.Parse(GO_BetNumLabel[focus_Index_Label].text);
        int focus_idx_betarray = LabelTextFindBetArrayIndex(focus_Value_Label);
        int max_label = GO_BetNumLabel.Length - 1;
        int max_betarray = betNumArray.Length - 1;
        m_idx_focus = focus_Index_Label;
        m_text_focus = focus_Value_Label.ToString();

        m_debug = "";
        // 以45度的UILabel為focus(亦即 delta is zero)，順時針為正遞增
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 1)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 1)].ToString();
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 2)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 2)].ToString();
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 3)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 3)].ToString();
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 4)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 4)].ToString();
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, -1)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, -1)].ToString();
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, -2)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, -2)].ToString();
        GO_BetNumLabel[GetSum(max_label, focus_Index_Label, -3)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, -3)].ToString();       

    }
    private int getBetNumFocusIndex()
    {

        for(int i = 0; i < GO_BetNumLabel.Length; i++)
        {
            if (GO_BetNumLabel[i].transform.eulerAngles.z > 67 && GO_BetNumLabel[i].transform.eulerAngles.z < 113)
                return i;
        }

        return -1;
    }

    private void NormalizeRotation()
    {
        float z_focus = GO_BET_PAD_NUM.transform.eulerAngles.z;
        int z_normal = 0;

        if (finalDragDelta.y > 0)
            z_normal = (((int)z_focus / 45) ) * 45;
        else
            z_normal = (((int)z_focus / 45) + 1) * 45;


        GO_BET_PAD_NUM.transform.eulerAngles = new Vector3(0, 0, z_normal);
    }

    IEnumerator AutoRun(int times,bool up)
    {
        while (times > 0)
        {
            if(up)
                addBetPadRotationZ(-50);
            else
                addBetPadRotationZ(50);

            times--;

            yield return new WaitForSeconds(0.02f);
        }

        NormalizeRotation();
    }
    private void continueRotate()
    {
        int mag = (int)(finalDragDelta.magnitude * 100);
        int callTimes = mag;
        Debug.Log(callTimes);

        callTimes = callTimes > 15 ? 15 : callTimes;
        if (finalDragDelta.y > 0)
            StartCoroutine(AutoRun(callTimes, true));
        else
            StartCoroutine(AutoRun(callTimes, false));
    }
    private int GetSum(int max , int idx_focus , int delta)
    {
        //max = betNumArray.Length - 1;

        int sum = idx_focus + delta;

        if (sum > max)
            sum = sum - max;
        else if (sum < 0)
            sum = sum + 1 + max;

        string str = "[" + idx_focus + "," + delta + "]=" + sum +"\n";
        m_debug += str;
        return sum;
    }
    private int LabelTextFindBetArrayIndex(int value)
    {
        for (int i = 0; i < betNumArray.Length; i++)
        {
            if (betNumArray[i] == value)
                return i;
        }
        return -1;
    }
}
