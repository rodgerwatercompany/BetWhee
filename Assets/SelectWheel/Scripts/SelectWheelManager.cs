using UnityEngine;
using System.Collections;

public class SelectWheelManager : MonoBehaviour {

    public Rodger.SelectWheelBase select_L;
    public Rodger.SelectWheelBase select_R;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onClick_SelectWheel_Left()
    {
        select_L.OnClick_SelectWheel();
    }
    public void onClick_SelectWheel_Right()
    {
        select_R.OnClick_SelectWheel();
    }
}
