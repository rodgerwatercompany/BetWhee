using UnityEngine;
using System.Collections;



public class TimerManager : BehaviourSingleton<TimerManager> {

	class TimerEvent{

		public TimerManager.VoidDelegate tickCallBack = null; 
		public int timerIndex;
		public float span;
		public float sumTime = 0.0f;
		public int callTimes = 0;
		public int targetTimes = 0;
		//public bool isLoop;
		public System.Object obj;
		
	}

	public delegate void VoidDelegate(object obj, int TimerIndex);
	static int idxCnt = 0;
	Hashtable TimerEventTable = new Hashtable();
	ArrayList removeList = new ArrayList();
	ArrayList addList = new ArrayList();

	private TimerManager(){
		//Debug.Log("Timer Cons");
		
	}

	override public void Awake(){
		base.Awake();
		go.name = "TimerManager"; 
		DontDestroyOnLoad(go);
	}


	public int addEvent(float span, int targetTimes, VoidDelegate callBackFunction , System.Object obj){

		TimerEvent temp = new TimerEvent();
		temp.span = span;
		temp.targetTimes = targetTimes;
		temp.timerIndex = idxCnt;
		temp.tickCallBack = callBackFunction;
		temp.obj = obj;
		addList.Add(temp);
		idxCnt++;
		return temp.timerIndex;
	
	}

	public void removeEvent(int index){
		removeList.Add(index);
	}

	public void removeAllEvent(){

		foreach(DictionaryEntry de in TimerEventTable){
			TimerEvent te = de.Value as TimerEvent;
			removeEvent(te.timerIndex);
		}

	}

	// Update is called once per frame
	void FixedUpdate () {

		foreach(DictionaryEntry de in TimerEventTable){
			TimerEvent te = de.Value as TimerEvent;
			//Debug.Log(te);
			te.sumTime += Time.deltaTime;

			if(te.sumTime >= te.span){

				if(removeList.Contains(te.timerIndex)){
					continue;
				}

				te.callTimes ++;
				te.tickCallBack(te.obj, te.timerIndex); 
				te.sumTime = 0.0f;
				//Debug.Log(te.callTimes + "   " + te.targetTimes);
				if(te.callTimes == te.targetTimes){
					removeEvent(te.timerIndex);
				}
			}

		}

		if(addList.Count != 0){
			for(int i = 0 ; i < addList.Count ; i++){
				TimerEvent temp = addList[i] as TimerEvent;
				TimerEventTable.Add(temp.timerIndex, temp);
			}
			addList.Clear();
		}
		
		if(removeList.Count != 0){
			for(int i = 0 ; i < removeList.Count ; i++){
				TimerEventTable.Remove(removeList[i]);
			}
			removeList.Clear();
		}
	}

	public void checkTiemrStatus(){

		Debug.Log("TimerEventTable  " + TimerEventTable.Count);
		Debug.Log("addList  " + addList.Count);
		Debug.Log("removeList  " + removeList.Count);

	}

	void OnDestroy() {

		TimerEventTable = new Hashtable();
		removeList = new ArrayList();
		addList = new ArrayList();
		idxCnt = 0;

	}



















}
