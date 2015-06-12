using UnityEngine;
using System.Collections;

namespace Rodger
{
    public class ColliderObject : MonoBehaviour
    {

        public delegate void FuncCB1(GameObject go);
        public delegate void FuncCB2(GameObject go, Vector2 delta);

        public FuncCB1 onDragStartCB;
        public FuncCB1 onDragEndCB;
        public FuncCB1 onDragOutCB;
        public FuncCB1 onDragOverCB;
        public FuncCB2 onDragCB;

        // Use this for initialization
        void Start()
        {
            UIEventListener.Get(gameObject).onDragStart = onDragStart;
            UIEventListener.Get(gameObject).onDragEnd = onDragEnd;
            UIEventListener.Get(gameObject).onDragOut = onDragOut;
            UIEventListener.Get(gameObject).onDragOver = onDragOver;
            UIEventListener.Get(gameObject).onDrag = onDrag;
        }

        void onDragStart(GameObject go)
        {
            onDragStartCB(go);
        }
        void onDragEnd(GameObject go)
        {
            onDragEndCB(go);
        }
        void onDragOut(GameObject go)
        {
            onDragOutCB(go);
        }
        void onDragOver(GameObject go)
        {
            onDragOverCB(go);
        }
        void onDrag(GameObject go, Vector2 delta)
        {
            onDragCB(go, delta);
        }        
    }
}