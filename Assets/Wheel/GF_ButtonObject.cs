using UnityEngine;

using System;
using System.Reflection;

namespace Game5835
{
    [System.Serializable]
    public struct Func
    {
        public GameObject target_GameObject;
        public string ComponentName;
        public string MethodName;
        public string Parameter;
    }

    [System.Serializable]
    public struct SpriteState
    {
        public UISprite uisprite;
        public string Normal;
        public string Pressed;
        public string Disabled;
    }

    [System.Serializable]
    public struct ButtonSound
    {
        public int idx_sound;
    }

    public class GF_ButtonObject : MonoBehaviour
    {
        UIButton[] uibuttons;
        public SoundManager SoundMgr;

        // 需要隨按鈕改變狀額外的圖片
        public SpriteState[] SpriteStates;

        public ButtonSound Button_Sound;

        // 要呼叫的方法
        public Func Func_normal;
        public Func Func_longpress;

        public bool bClickDisable;


        // OnPress
        private bool haveDone;   // 防止長按之後，進行短按。
        private bool bpressing;  // 防止短按之後，進行長按。
        private float fpresstime_first;

        void Awake()
        {
            uibuttons = GetComponents<UIButton>();
        }

        void Update()
        {
            if (bpressing && !haveDone)
            {
                if (Time.time - fpresstime_first > 2.0f)
                {
                    if (!string.IsNullOrEmpty(Func_longpress.MethodName))
                    {
                        //this.CallLua(luaFunc_longpress.GameObject,luaFunc_longpress.MethodName, luaFunc_longpress.parameter);
                        haveDone = true;
                        SetState("Disabled");

                        InvokeFun("LongPress");
                    }
                }
            }
        }

        void OnEnable()
        {
            bpressing = false;
            haveDone = false;
        }
        /*
        public void SetActive(bool sw)
        {

            gameObject.SetActive(sw);

            if (sw)
            {
                SetState("Normal");
            }
        }*/

        public void SetState(string state)
        {
            //Global.print(gameObject.name + " SetState " + state + "\n");
            switch (state)
            {
                case "Normal":
                    SetSprite("Normal");
                    gameObject.SetActive(true);
                    for (int i = 0; i < uibuttons.Length; i++)
                    {
                        uibuttons[i].enabled = true;
                        uibuttons[i].SetState(UIButtonColor.State.Normal, false);
                    }
                    break;
                case "Hover":
                    for (int i = 0; i < uibuttons.Length; i++)
                    {
                        uibuttons[i].SetState(UIButtonColor.State.Hover, false);
                    }
                    break;
                case "Pressed":
                    for (int i = 0; i < uibuttons.Length; i++)
                    {
                        uibuttons[i].SetState(UIButtonColor.State.Pressed, false);
                    }
                    break;
                case "Disabled":
                    SetSprite("Disabled");
                    for (int i = 0; i < uibuttons.Length; i++)
                    {
                        uibuttons[i].enabled = false;
                        uibuttons[i].SetState(UIButtonColor.State.Disabled, false);
                    }
                    break;
                case "OFF":
                    gameObject.SetActive(false);
                    break;
            }
        }

        public void OnClick()
        {
            if (uibuttons[0].state != UIButtonColor.State.Disabled && uibuttons[0].enabled && !haveDone)
            {
                if (SoundMgr != null)
                {
                    SoundMgr.Play(Button_Sound.idx_sound, false);
                }

                //haveDone = true;

                if (bClickDisable)
                    SetState("Disabled");

                if (!string.IsNullOrEmpty(Func_normal.MethodName))
                {
                    InvokeFun("NormalPress");
                }
                //this.CallLua(luaFunc_normal.GameObject, luaFunc_normal.MethodName, luaFunc_normal.parameter);
            }
        }

        public void OnPress()
        {
            // 按下
            if (bpressing)
            {
                SetSprite("Normal");
                //print("close");
                bpressing = false;
            }
            // 彈起
            else
            {
                SetSprite("Pressed");
                //print("open");
                bpressing = true;

                fpresstime_first = Time.time;
            }
        }

        public void SetSpriteStatesName(int idx, string normal, string pressed, string disabled)
        {
            SpriteStates[idx].Normal = normal;
            SpriteStates[idx].Pressed = pressed;
            SpriteStates[idx].Disabled = disabled;

            SetSprite("Normal");
        }

        private void CallLua(string lua_gameObject, string lua_methodname, string str_parms)
        {
            /*
            if(string.IsNullOrEmpty(lua_gameObject))
            {
                if (!string.IsNullOrEmpty(str_parms))
                    LuaManager_new.Instance().CallLuaFuction(lua_methodname, str_parms);
                else
                    LuaManager_new.Instance().CallLuaFuction(lua_methodname);
            }
            else
            {
                LuaManager_new.Instance().CallLuaGameObject(lua_gameObject, lua_methodname, str_parms);
            }*/
        }

        private void InvokeFun(string str)
        {
            if (str == "LongPress")
            {
                object obj = Func_longpress.target_GameObject.GetComponent(Func_longpress.ComponentName);
                Type thisType = Func_longpress.target_GameObject.GetComponent(Func_longpress.ComponentName).GetType();
                MethodInfo theMethod = thisType.GetMethod(Func_longpress.MethodName);

                if (string.IsNullOrEmpty(Func_longpress.Parameter))
                    theMethod.Invoke(obj, null);
                else
                    theMethod.Invoke(obj, new object[] { Func_longpress.Parameter });
            }
            else if (str == "NormalPress")
            {
                object obj = Func_normal.target_GameObject.GetComponent(Func_normal.ComponentName);
                Type thisType = Func_normal.target_GameObject.GetComponent(Func_normal.ComponentName).GetType();
                MethodInfo theMethod = thisType.GetMethod(Func_normal.MethodName);

                if (string.IsNullOrEmpty(Func_normal.Parameter))
                    theMethod.Invoke(obj, null);
                else
                    theMethod.Invoke(obj, new object[] { Func_normal.Parameter });

            }
        }

        private void SetSprite(string state)
        {
            if (SpriteStates.Length > 0)
            {
                for (int i = 0; i < SpriteStates.Length; i++)
                {
                    if (state == "Normal")
                        SpriteStates[i].uisprite.spriteName = SpriteStates[i].Normal;
                    else if (state == "Pressed")
                        SpriteStates[i].uisprite.spriteName = SpriteStates[i].Pressed;
                    else if (state == "Disabled")
                        SpriteStates[i].uisprite.spriteName = SpriteStates[i].Disabled;
                }
            }
        }
    }
}