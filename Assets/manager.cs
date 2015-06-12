using UnityEngine;
using System.Collections;

using Rodger;

public class manager : MonoBehaviour {

    public BetWheel betWheel;

	// Use this for initialization
	void Start () {

        betWheel.onChageBetNumCB = OnBetNumChange;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if(GUILayout.Button("Hide"))
        {
            betWheel.HideBetButton();
        }
        if (GUILayout.Button("Show"))
        {
            betWheel.ShowBetButton();
        }
    }
    void OnBetNumChange(int value)
    {
        print("New bet num is " + value);
    }
}
