using UnityEngine;
using System.Collections;

namespace Game5095
{
    public class SelectWheel_R : Rodger.SelectWheelBase
    {
        private int AUTO_HIDE_THRESHOLD_SEC = 3;

        private float m_countAutoHide;

        private bool m_isPress;

        public Rodger.VOIDCB onCountAutoHideResetCB;

        protected override void Awake()
        {
            base.Awake();

            selectNumArray = new int[20]
            {
                1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
            };
        }
        protected override void Start()
        {
            base.Start();
            m_countAutoHide = 0;
            m_isPress = false;

            //G_5095.m_selectWheel_Left.onCountAutoHideResetCB = OnAutoHideReset;
        }
        protected override void Update()
        {
            base.Update();

            #region AutoHide
            if (!m_isHideNow && !m_isPress)
            {
                m_countAutoHide += Time.deltaTime;

                if (m_countAutoHide >= AUTO_HIDE_THRESHOLD_SEC)
                {
                    ResetAutoHideCount();
                    //HideWheel();
                    //G_5095.GameManager5095.CloseSelectWheel();
                }
            }
            else if (m_isPress)
                ResetAutoHideCount();
            #endregion
        }
        private void OnAutoHideReset()
        {
            m_countAutoHide = 0;
        }
        protected override IEnumerator HideOrShowWheel()
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

                            //GO_CloseCollider.SetActive(true);
                            if (onOpenCloseWheelCB != null)
                                onOpenCloseWheelCB(true);

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

                            //m_colliderObj.gameObject.SetActive(false);

                            //GO_CloseCollider.SetActive(false);
                            if (onOpenCloseWheelCB != null)
                                onOpenCloseWheelCB(false);

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

                            //GO_CloseCollider.SetActive(true);
                            onOpenCloseWheelCB(true);

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
                            trans_others.localPosition = new Vector3(55, 0, 0);

                            //m_colliderObj.gameObject.SetActive(false);

                            //GO_CloseCollider.SetActive(false);
                            onOpenCloseWheelCB(false);

                            m_isHideNow = true;
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }

            }

            m_allowClick = true;
        }

        public override void HideWheel()
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
                    // GO_CloseCollider.SetActive(false);
                }
            }
        }

        public override void OnClick_SelectWheel()
        {
            if (m_allowClick)
            {
                m_allowClick = false;
                int idx_focus = getSelectNumFocusIndex();
                if (onChageSelectNumCB != null)
                    onChageSelectNumCB(int.Parse(GO_SelectNumLabel[idx_focus].text));

                StartCoroutine(HideOrShowWheel());
            }
            ResetAutoHideCount();
        }
        protected override void onDrag(GameObject go, Vector2 delta)
        {
            base.onDrag(go, delta);
            ResetAutoHideCount();
        }
        protected override void onPress(GameObject go, bool state)
        {
            m_isPress = state;
        }
        private void ResetAutoHideCount()
        {
            if (onCountAutoHideResetCB != null)
                onCountAutoHideResetCB();

            m_countAutoHide = 0;
        }
    }
}