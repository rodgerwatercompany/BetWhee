using UnityEngine;

namespace Rodger
{
    public class ColliderObject : MonoBehaviour
    {

        public delegate void FuncCB1(GameObject go);
        public delegate void FuncCB2(GameObject go, Vector2 delta);
        public delegate void FuncCB3(GameObject go, bool state);

        public FuncCB1 onDragStartCB;
        public FuncCB1 onDragEndCB;
        public FuncCB1 onDragOutCB;
        public FuncCB1 onDragOverCB;
        public FuncCB2 onDragCB;
        public FuncCB3 onPressCB;
        public FuncCB1 onClickCB;
        public FuncCB3 onSelectCB;

        // Use this for initialization
        void Start()
        {
            UIEventListener.Get(gameObject).onDragStart = onDragStart;
            UIEventListener.Get(gameObject).onDragEnd = onDragEnd;
            UIEventListener.Get(gameObject).onDragOut = onDragOut;
            UIEventListener.Get(gameObject).onDragOver = onDragOver;
            UIEventListener.Get(gameObject).onDrag = onDrag;
            UIEventListener.Get(gameObject).onPress = onPress;
            UIEventListener.Get(gameObject).onClick = onClick;
            UIEventListener.Get(gameObject).onSelect = onSelect;
        }

        void onDragStart(GameObject go)
        {
            if (onDragStartCB != null)
                onDragStartCB(go);
        }
        void onDragEnd(GameObject go)
        {
            if (onDragEndCB != null)
                onDragEndCB(go);
        }
        void onDragOut(GameObject go)
        {
            if (onDragOutCB != null)
                onDragOutCB(go);
        }
        void onDragOver(GameObject go)
        {
            if (onDragOverCB != null)
                onDragOverCB(go);
        }
        void onDrag(GameObject go, Vector2 delta)
        {
            if (onDragCB != null)
                onDragCB(go, delta);
        }
        void onPress(GameObject go, bool state)
        {
            if (onPressCB != null)
                onPressCB(go, state);
        }
        void onClick(GameObject go)
        {
            if (onClickCB != null)
                onClickCB(go);
        }
        void onSelect(GameObject go, bool state)
        {
            if (onSelectCB != null)
                onSelectCB(go, state);
        }
    }
}