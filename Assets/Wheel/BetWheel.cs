using UnityEngine;
using System.Collections;

namespace Rodger
{
    public class BetWheel : MonoBehaviour
    {
        readonly int[] betNumArray =
        {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
        };
        public delegate void FuncCB1(int value);

        public FuncCB1 onChageBetNumCB;

        public GameObject GO_Wheel;
        public GameObject GO_Others;
        public GameObject GO_CloseCollider;
        public UIButton But_BetWheel;

        public UILabel[] GO_BetNumLabel;


        Vector2 finalDragDelta;

        ColliderObject m_colliderObj;


        int last_idx_focus_Label = -1;
        bool m_allowClick;
        bool m_isHideNow;

        public UIFont UIFont_normal;
        public UIFont UIFont_focus;
        

        void Awake()
        {
            m_colliderObj = GetComponentInChildren<ColliderObject>();
            m_colliderObj.onDragStartCB = onDragStart;
            m_colliderObj.onDragEndCB = onDragEnd;
            m_colliderObj.onDragOutCB = onDragOut;
            m_colliderObj.onDragOverCB = onDragOver;
            m_colliderObj.onDragCB = onDrag;
        }
        // Use this for initialization
        void Start()
        {
            SetBetNumDirectly(1,false);

            m_allowClick = true;
            m_isHideNow = true;
            
            m_colliderObj.gameObject.SetActive(false);
            GO_CloseCollider.SetActive(false);
            GO_Wheel.transform.localPosition = new Vector3(330, 0, 0);
            GO_Others.transform.localPosition = new Vector3(330, 0, 0);

        }

        void onDragStart(GameObject go)
        {
        }
        void onDragEnd(GameObject go)
        {
            continueRotate();
        }
        void onDragOut(GameObject go)
        {
        }
        void onDragOver(GameObject go)
        {
        }
        void onDrag(GameObject go, Vector2 delta)
        {
            delta = delta / Screen.width;
            finalDragDelta = delta;

            float length = delta.magnitude * 1000;

            if (delta.y > 0)
                addBetPadRotationZ(length * (-1));
            else if (delta.y < 0)
                addBetPadRotationZ(length);
        }

        public void OnClick_BetWheel()
        {
            if(m_allowClick)
            {
                m_allowClick = false;
                int idx_focus = getBetNumFocusIndex();
                onChageBetNumCB(int.Parse(GO_BetNumLabel[idx_focus].text));
                m_colliderObj.gameObject.SetActive(false);
                GO_CloseCollider.SetActive(false);
                StartCoroutine(HideOrShowBetWheel());                
            }
        }
        public void OnClick_MaxBet()
        {
            SetBetNumDirectly(betNumArray[betNumArray.Length - 1], true);
            ShowBetWheel();
        }
        public void ShowBetWheel()
        {
            if (m_allowClick)
            {
                m_allowClick = false;

                m_colliderObj.gameObject.SetActive(false);
                GO_CloseCollider.SetActive(false);
                
                if (m_isHideNow)
                    StartCoroutine(HideOrShowBetWheel());
                else
                {
                    m_colliderObj.gameObject.SetActive(true);
                    GO_CloseCollider.SetActive(true);
                }
            }
        }
        public void HideBetWheel()
        {
            if (m_allowClick)
            {
                m_allowClick = false;

                m_colliderObj.gameObject.SetActive(false);
                GO_CloseCollider.SetActive(false);

                if (!m_isHideNow)
                    StartCoroutine(HideOrShowBetWheel());
            }
        }
        public void ShowBetButton()
        {
            if(!But_BetWheel.enabled)
            {
                StartCoroutine(HideOrShowBetButton(false));
            }
        }
        public void HideBetButton()
        {
            if (But_BetWheel.enabled)
            {
                HideBetWheel();
                But_BetWheel.enabled = false;
                StartCoroutine(HideOrShowBetButton(true));
            }
        }

        #region Wheel Display
        private void addBetPadRotationZ(float value)
        {
            value *= 0.3f;

            float ori_z = GO_Wheel.transform.eulerAngles.z;

            GO_Wheel.transform.eulerAngles = new Vector3(0, 0, ori_z + value);

            UpdateBetLabel();
        }
        // 更新 UILabel 顯示的下注值
        private void UpdateBetLabel()
        {

            int focus_Index_Label = getBetNumFocusIndex();

            if (last_idx_focus_Label != focus_Index_Label)
            {
                last_idx_focus_Label = focus_Index_Label;

                int focus_Value_Label = int.Parse(GO_BetNumLabel[focus_Index_Label].text);
                int focus_idx_betarray = LabelTextFindBetArrayIndex(focus_Value_Label);
                int max_label = GO_BetNumLabel.Length - 1;
                int max_betarray = betNumArray.Length - 1;


                // 以45度的UILabel為focus(亦即 delta is zero)，逆時針為正遞增
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 1)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 1)].ToString();
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 2)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 2)].ToString();
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 3)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 3)].ToString();
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, 4)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, 4)].ToString();
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, -1)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, -1)].ToString();
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, -2)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, -2)].ToString();
                GO_BetNumLabel[GetSum(max_label, focus_Index_Label, -3)].text = betNumArray[GetSum(max_betarray, focus_idx_betarray, -3)].ToString();

                // Font Color
                for (int i = 0; i < GO_BetNumLabel.Length; i++)
                {
                    if (i == last_idx_focus_Label)
                        GO_BetNumLabel[i].bitmapFont = UIFont_focus;
                    else
                        GO_BetNumLabel[i].bitmapFont = UIFont_normal;
                }
            }
        }
        private int getBetNumFocusIndex()
        {
            for (int i = 0; i < GO_BetNumLabel.Length; i++)
            {
                if (GO_BetNumLabel[i].transform.eulerAngles.z > 67 && GO_BetNumLabel[i].transform.eulerAngles.z < 113)
                    return i;
            }
            return -1;
        }
        private void continueRotate()
        {
            int mag = (int)(finalDragDelta.magnitude * 100);
            int callTimes = mag;

            callTimes = callTimes > 15 ? 15 : callTimes;
            if (finalDragDelta.y > 0)
                StartCoroutine(AutoRun(callTimes, true));
            else
                StartCoroutine(AutoRun(callTimes, false));
        }
        IEnumerator AutoRun(int times, bool up)
        {
            while (times > 0)
            {
                if (up)
                    addBetPadRotationZ(-50);
                else
                    addBetPadRotationZ(50);

                times--;

                yield return new WaitForSeconds(0.02f);
            }

            NormalizeRotation();
        }
        private void NormalizeRotation()
        {
            int idx_focus = getBetNumFocusIndex();
            float z_focus = GO_BetNumLabel[idx_focus].transform.eulerAngles.z;

            float z_pad = GO_Wheel.transform.eulerAngles.z;
            int z_normal = 0;
            if (z_focus - 90 > 0)
                z_normal = (((int)z_pad / 45)) * 45;
            else
                z_normal = (((int)z_pad / 45) + 1) * 45;

            GO_Wheel.transform.eulerAngles = new Vector3(0, 0, z_normal);

            //idx_focus = getBetNumFocusIndex();

            onChageBetNumCB(int.Parse(GO_BetNumLabel[idx_focus].text));
        }
        private void SetBetNumDirectly(int betnum,bool b_onChangeBetNumCB)
        {
            int idx_focus_Label = getBetNumFocusIndex();
            int idx_focus_BetNum = LabelTextFindBetArrayIndex(betnum);
            int max_label = GO_BetNumLabel.Length - 1;
            int max_betarray = betNumArray.Length - 1;
            GO_BetNumLabel[idx_focus_Label].text = betNumArray[idx_focus_BetNum].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, 1)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, 1)].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, 2)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, 2)].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, 3)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, 3)].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, 4)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, 4)].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, -1)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, -1)].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, -2)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, -2)].ToString();
            GO_BetNumLabel[GetSum(max_label, idx_focus_Label, -3)].text = betNumArray[GetSum(max_betarray, idx_focus_BetNum, -3)].ToString();

            if(b_onChangeBetNumCB)
                onChageBetNumCB(betnum);
        }
        private int GetSum(int max, int idx_focus, int delta)
        {
            int sum = idx_focus + delta;

            if (sum > max)
                sum = sum - (max + 1);
            else if (sum < 0)
                sum = sum + 1 + max;

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
        #endregion

        #region Move
        IEnumerator HideOrShowBetWheel()
        {
            Transform trans_betwheel = GO_Wheel.transform;
            Transform trans_others = GO_Others.transform;
            float speed = 0;
            while(true)
            {
                speed = 1.5f * Time.deltaTime;
                if (m_isHideNow)
                {
                    trans_betwheel.Translate(-speed, 0, 0, Space.World);
                    trans_others.Translate(-speed, 0, 0, Space.World);
                    if (trans_betwheel.localPosition.x <= 0)
                    {
                        trans_betwheel.localPosition = Vector3.zero;
                        trans_others.localPosition = Vector3.zero;

                        m_colliderObj.gameObject.SetActive(true);
                        GO_CloseCollider.SetActive(true);

                        m_isHideNow = false;
                        break;
                    }
                    yield return new WaitForEndOfFrame();

                }
                else
                {
                    trans_betwheel.Translate(speed, 0, 0, Space.World);
                    trans_others.Translate(speed, 0, 0, Space.World);
                    if (trans_betwheel.localPosition.x >= 330)
                    {
                        trans_betwheel.localPosition = new Vector3(330, 0, 0);
                        trans_others.localPosition = new Vector3(330, 0, 0);

                        m_colliderObj.gameObject.SetActive(false);
                        GO_CloseCollider.SetActive(false);

                        m_isHideNow = true;
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }

            m_allowClick = true;
        }
        IEnumerator HideOrShowBetButton(bool willhide)
        {
            Transform trans_butbetwheel = But_BetWheel.gameObject.transform;
            float speed = 0;
            while (true)
            {
                speed = 1.5f * Time.deltaTime;
                if (willhide)
                {
                    trans_butbetwheel.Translate(speed, 0, 0, Space.World);
                    if(trans_butbetwheel.localPosition.x >= 100)
                    {
                        trans_butbetwheel.localPosition = new Vector3(100, 0, 0);
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    trans_butbetwheel.Translate(-speed, 0, 0, Space.World);
                    if (trans_butbetwheel.localPosition.x <= -54)
                    {
                        trans_butbetwheel.localPosition = new Vector3(-54, 0, 0);
                        But_BetWheel.enabled = true;
                        m_allowClick = true;
                        break;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
        }
        #endregion
    }
}