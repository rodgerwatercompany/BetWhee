using UnityEngine;
using System.Collections;

public class BehaviourSingleton <T> : MonoBehaviour where T : MonoBehaviour {

	static protected T m_instance;
	static protected GameObject go = null;

	public virtual void Awake(){ 
		go = this.gameObject;
	}

	public static T instance {
		get{
			if (m_instance == null){
				if(go == null){
					go = new GameObject();
					m_instance = go.AddComponent<T>();
					Debug.LogWarning("A Gameobject named" + go.name + " is created by BehaviourSingleton!");
				}
				else{
					m_instance = go.GetComponent<T>();
				}
			}
			return m_instance;
		}
	}

}
