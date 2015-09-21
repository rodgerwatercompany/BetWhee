using UnityEngine;
using System.Collections;


namespace Rodger
{
    public abstract class SelectWheelBase : MonoBehaviour
    {
        public enum SelectSide
        {
            LEFT_SIDE,
            RIGHT_SIDE,
        }
        
        protected int[] selectNumArray;
        
        public SelectSide m_selectside;

        public VOIDintCB onChageSelectNumCB;
        public VOIDBOOLCB onOpenCloseWheelCB;
        public VOIDCB onShowWheelButtonFinish;

        public GameObject GO_Wheel;
        public GameObject GO_Others;
        public GameObject GO_CloseCollider;
        public UIButton But_Wheel;

        public UILabel[] GO_SelectNumLabel;


        protected Vector2 finalDragDelta;

        protected ColliderObject m_colliderObj;


        protected int last_idx_focus_Label = -1;
        protected bool m_allowClick;
        protected bool m_wheelMoving;

        protected bool m_isHideNow;
        
        public UIFont UIFont_normal;
        public UIFont UIFont_focus;

        protected bool m_allowDrag;

        public bool IsButtonEnabled()
        {
            return But_Wheel.enabled;
        }

        protected virtual void Awake()
        {
            m_colliderObj = GetComponentInChildren<ColliderObject>();
            m_colliderObj.onDragStartCB = onDragStart;
            m_colliderObj.onDragEndCB = onDragEnd;
            m_colliderObj.onDragOutCB = onDragOut;
            m_colliderObj.onDragOverCB = onDragOver;
            m_colliderObj.onDragCB = onDrag;
            m_colliderObj.onPressCB = onPress;
            //m_colliderObj.onSelectCB = onSelect;

            /*
            selectNumArray = new int[]{
                1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
            };*/
        }
        // Use this for initialization
        protected virtual void Start()
        {
            SetSelectNumDirectly(1,false);

            m_allowClick = true;
            m_wheelMoving = false;
            m_isHideNow = true;
            
            m_colliderObj.gameObject.SetActive(false);
            GO_CloseCollider.SetActive(false);

            if (m_selectside == SelectSide.RIGHT_SIDE)
            {
                GO_Wheel.transform.localPosition = new Vector3(330, 0, 0);
                GO_Others.transform.localPosition = new Vector3(330, 0, 0);
            }
            else
            {
                GO_Wheel.transform.localPosition = new Vector3(-330, 0, 0);
                GO_Others.transform.localPosition = new Vector3( 65, 0, 0);
            }

            m_allowDrag = false;

        }
        protected virtual void Update()
        {
        }

        protected virtual void onDragStart(GameObject go)
        {
            m_allowDrag = true;
        }
        protected virtual void onDragEnd(GameObject go)
        {

            if (m_allowDrag)
            {
                m_allowDrag = false;
                continueRotate();
            }
        }
        protected virtual void onDragOut(GameObject go)
        {/*
            if (m_allowDrag)
            {
                m_allowDrag = false;
                continueRotate();
            }*/
        }
        protected virtual void onDragOver(GameObject go)
        {
        }
        protected virtual void onDrag(GameObject go, Vector2 delta)
        {
            if (m_allowDrag)
            {
                m_wheelMoving = true;

                delta = delta / Screen.width;
                finalDragDelta = delta;

                float length = delta.magnitude * 1500;

                if (m_selectside == SelectSide.RIGHT_SIDE)
                {
                    if (delta.y > 0)
                        addBetPadRotationZ(length * (-1));
                    else if (delta.y < 0)
                        addBetPadRotationZ(length);
                    else
                        addBetPadRotationZ(0);
                }
                else
                {
                    if (delta.y > 0)
                        addBetPadRotationZ(length);
                    else if (delta.y < 0)
                        addBetPadRotationZ(length * (-1));
                    else
                        addBetPadRotationZ(0);

                }
            }
        }
        protected virtual void onPress(GameObject go,bool state)
        {

        }
        protected virtual void onSelect(GameObject go,bool state)
        {
        }
        public virtual void OnClick_SelectWheel()
        {
            if(m_allowClick)
            {
                m_allowClick = false;
                int idx_focus = getSelectNumFocusIndex();
                if(onChageSelectNumCB != null)
                    onChageSelectNumCB(int.Parse(GO_SelectNumLabel[idx_focus].text));
                m_colliderObj.gameObject.SetActive(false);
                //GO_CloseCollider.SetActive(false);
                StartCoroutine(HideOrShowWheel());                
            }
        }
        
        public virtual void OnClick_MaxSelect()
        {
            SetSelectNumDirectly(selectNumArray[selectNumArray.Length - 1], true);
            //ShowWheel();
        }
        public virtual void ShowWheel()
        {
            if (m_allowClick)
            {
                m_allowClick = false;

                m_colliderObj.gameObject.SetActive(false);
                GO_CloseCollider.SetActive(false);
                
                if (m_isHideNow)
                    StartCoroutine(HideOrShowWheel());
                else
                {
                    m_colliderObj.gameObject.SetActive(true);
                    GO_CloseCollider.SetActive(true);
                }
            }
        }
        public virtual void HideWheel()
        {
            if (m_allowClick && !m_wheelMoving)
            {
                m_allowClick = false;

                m_colliderObj.gameObject.SetActive(false);

                if (!m_isHideNow)
                    StartCoroutine(HideOrShowWheel());
                else
                {
                    m_allowClick = true;
                    GO_CloseCollider.SetActive(false);
                }                
            }
            
        }
        public virtual void ShowButton()
        {
            if (!But_Wheel.enabled)
            {
                StartCoroutine(HideOrShowButton(false));
            }
        }
        public virtual void HideButton()
        {
            Rodger.Global.print("HideButton But_Wheel.enabled " + But_Wheel.enabled);

            if (But_Wheel.enabled)
            {
                HideWheel();
                But_Wheel.enabled = false;
                StartCoroutine(HideOrShowButton(true));
            }
        }

        #region Wheel Display
        private void addBetPadRotationZ(float value)
        {
            value *= 0.3f;

            float ori_z = GO_Wheel.transform.eulerAngles.z;

            GO_Wheel.transform.eulerAngles = new Vector3(0, 0, ori_z + value);

            UpdateSelectLabel();
        }
        // 更新 UILabel 顯示的下注值
        private void UpdateSelectLabel()
        {
            int focus_Index_Label = getSelectNumFocusIndex();

            Global.print("Focus Number " + GO_SelectNumLabel[focus_Index_Label].text);

            if (last_idx_focus_Label != focus_Index_Label)
            {
                last_idx_focus_Label = focus_Index_Label;

                int focus_Value_Label = int.Parse(GO_SelectNumLabel[focus_Index_Label].text);
                int focus_idx_betarray = LabelTextFindBetArrayIndex(focus_Value_Label);
                int max_label = GO_SelectNumLabel.Length - 1;
                int max_betarray = selectNumArray.Length - 1;

                if (m_selectside == SelectSide.RIGHT_SIDE)
                {

                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 1)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 2)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 3)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 4)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 4)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, -1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -1)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, -2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -2)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, -3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -3)].ToString("00");
                }
                else
                {
                    // 以45度的UILabel為focus(亦即 delta is zero)，逆時針為正遞增
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -1)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -2)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -3)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, 4)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, -4)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, -1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 1)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, -2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 2)].ToString("00");
                    GO_SelectNumLabel[Math.Cyclic_Summation(max_label, focus_Index_Label, -3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, focus_idx_betarray, 3)].ToString("00");
                }
                // Font Color
                for (int i = 0; i < GO_SelectNumLabel.Length; i++)
                {
                    if (i == last_idx_focus_Label)
                    {
                        GO_SelectNumLabel[i].bitmapFont = UIFont_focus;
                        Global.print("Select Number " + GO_SelectNumLabel[i].text);
                    }
                    else
                        GO_SelectNumLabel[i].bitmapFont = UIFont_normal;
                }
            }
        }
        protected int getSelectNumFocusIndex()
        {
            for (int i = 0; i < GO_SelectNumLabel.Length; i++)
            {
                float z = GO_SelectNumLabel[i].transform.eulerAngles.z;
                z = Math.Cyclic_Summation(360, z, 90);
                //if (GO_BetNumLabel[i].transform.eulerAngles.z > 67 && GO_BetNumLabel[i].transform.eulerAngles.z < 113)
                if (z > 67 && z < 113)
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

                if (m_selectside == SelectSide.RIGHT_SIDE)
                {
                    if (up)
                        addBetPadRotationZ(-50);
                    else
                        addBetPadRotationZ(50);
                }
                else
                {
                    if (up)
                        addBetPadRotationZ(50);
                    else
                        addBetPadRotationZ(-50);
                }

                times--;

                yield return new WaitForSeconds(0.02f);
            }

            NormalizeRotation();
        }
        private void NormalizeRotation()
        {
            int idx_focus = getSelectNumFocusIndex();
            float z_focus = GO_SelectNumLabel[idx_focus].transform.eulerAngles.z;
            z_focus = Math.Cyclic_Summation(360, z_focus, 90);
            //float z_focus = GO_BetNumLabel[idx_focus].transform.eulerAngles.z;

            float z_pad = GO_Wheel.transform.eulerAngles.z;
            int z_normal = 0;
            if (z_focus - 90 > 0)
                z_normal = (((int)z_pad / 45)) * 45;
            else
                z_normal = (((int)z_pad / 45) + 1) * 45;

            GO_Wheel.transform.eulerAngles = new Vector3(0, 0, z_normal);
            

            //idx_focus = getBetNumFocusIndex();
            if (onChageSelectNumCB != null)
                onChageSelectNumCB(int.Parse(GO_SelectNumLabel[idx_focus].text));

            m_wheelMoving = false;
        }
        private void SetSelectNumDirectly(int betnum,bool b_onChangeBetNumCB)
        {
            int idx_focus_Label = getSelectNumFocusIndex();
            int idx_focus_BetNum = LabelTextFindBetArrayIndex(betnum);
            int max_label = GO_SelectNumLabel.Length - 1;
            int max_betarray = selectNumArray.Length - 1;
            GO_SelectNumLabel[idx_focus_Label].text = selectNumArray[idx_focus_BetNum].ToString("00");

            if (m_selectside == SelectSide.RIGHT_SIDE)
            {
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 1)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 2)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 3)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 4)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 4)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, -1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -1)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, -2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -2)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, -3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -3)].ToString("00");

            }
            else
            {
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -1)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -2)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -3)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, 4)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, -4)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, -1)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 1)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, -2)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 2)].ToString("00");
                GO_SelectNumLabel[Math.Cyclic_Summation(max_label, idx_focus_Label, -3)].text = selectNumArray[Math.Cyclic_Summation(max_betarray, idx_focus_BetNum, 3)].ToString("00");
            }

            // Font Color
            for (int i = 0; i < GO_SelectNumLabel.Length; i++)
            {
                if (i == idx_focus_Label)
                    GO_SelectNumLabel[i].bitmapFont = UIFont_focus;
                else
                    GO_SelectNumLabel[i].bitmapFont = UIFont_normal;
            }

            if (b_onChangeBetNumCB)
                if (onChageSelectNumCB != null)
                    onChageSelectNumCB(betnum);
        }
        private int LabelTextFindBetArrayIndex(int value)
        {
            for (int i = 0; i < selectNumArray.Length; i++)
            {
                if (selectNumArray[i] == value)
                    return i;
            }
            return -1;
        }
        #endregion

        #region Move
        protected virtual IEnumerator HideOrShowWheel()
        {
            Transform trans_wheel = GO_Wheel.transform;
            Transform trans_others = GO_Others.transform;
            float speed = 0;

            if (m_selectside == SelectSide.RIGHT_SIDE)
            {
                while (true)
                {
                    speed = 1.5f * Time.deltaTime;
                    if (m_isHideNow)
                    {
                        trans_wheel.Translate(-speed, 0, 0, Space.World);
                        trans_others.Translate(-speed, 0, 0, Space.World);
                        if (trans_wheel.localPosition.x <= 0)
                        {
                            trans_wheel.localPosition = Vector3.zero;
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
                        trans_wheel.Translate(speed, 0, 0, Space.World);
                        trans_others.Translate(speed, 0, 0, Space.World);
                        if (trans_wheel.localPosition.x >= 330)
                        {
                            trans_wheel.localPosition = new Vector3(330, 0, 0);
                            trans_others.localPosition = new Vector3(330, 0, 0);

                            m_colliderObj.gameObject.SetActive(false);
                            GO_CloseCollider.SetActive(false);

                            m_isHideNow = true;
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            else
            {
                while (true)
                {
                    speed = 1.5f * Time.deltaTime;
                    if (m_isHideNow)
                    {
                        trans_wheel.Translate(speed, 0, 0, Space.World);
                        trans_others.Translate(speed, 0, 0, Space.World);
                        if (trans_wheel.localPosition.x >= 0)
                        {
                            trans_wheel.localPosition = Vector3.zero;
                            trans_others.localPosition = new Vector3(390, 0, 0);

                            m_colliderObj.gameObject.SetActive(true);
                            GO_CloseCollider.SetActive(true);

                            m_isHideNow = false;
                            break;
                        }
                        yield return new WaitForEndOfFrame();

                    }
                    else
                    {
                        trans_wheel.Translate(-speed, 0, 0, Space.World);
                        trans_others.Translate(-speed, 0, 0, Space.World);
                        if (trans_wheel.localPosition.x <= -330)
                        {
                            trans_wheel.localPosition = new Vector3(-330, 0, 0);
                            trans_others.localPosition = new Vector3( 55, 0, 0);

                            m_colliderObj.gameObject.SetActive(false);
                            GO_CloseCollider.SetActive(false);

                            m_isHideNow = true;
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }

            }

            m_allowClick = true;
        }
        IEnumerator HideOrShowButton(bool willhide)
        {
            Transform trans_butwheel = But_Wheel.gameObject.transform;
            float speed = 0;
            while (true)
            {
                speed = 1.5f * Time.deltaTime;
                if (m_selectside == SelectSide.RIGHT_SIDE)
                {
                    if (willhide)
                    {
                        trans_butwheel.Translate(speed, 0, 0, Space.World);
                        if (trans_butwheel.localPosition.x >= 100)
                        {
                            trans_butwheel.localPosition = new Vector3(100, 0, 0);
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        trans_butwheel.Translate(-speed, 0, 0, Space.World);
                        if (trans_butwheel.localPosition.x <= -54)
                        {
                            trans_butwheel.localPosition = new Vector3(-54, 0, 0);
                            But_Wheel.enabled = true;
                            m_allowClick = true;

                            if(onShowWheelButtonFinish != null)
                                onShowWheelButtonFinish();
                            break;
                        }

                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    if (willhide)
                    {
                        trans_butwheel.Translate(-speed, 0, 0, Space.World);
                        if (trans_butwheel.localPosition.x <= -208)
                        {
                            trans_butwheel.localPosition = new Vector3(-208, 0, 0);
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        trans_butwheel.Translate(speed, 0, 0, Space.World);
                        if (trans_butwheel.localPosition.x >= -54)
                        {
                            trans_butwheel.localPosition = new Vector3(-54, 0, 0);
                            But_Wheel.enabled = true;
                            m_allowClick = true;

                            if (onShowWheelButtonFinish != null)
                                onShowWheelButtonFinish();

                            break;
                        }

                        yield return new WaitForEndOfFrame();
                    }

                }
            }
        }
        #endregion
    }
}