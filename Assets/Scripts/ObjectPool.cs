using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {

	public static ObjectPool Instance;
	public List<Pool> pools;
	public Dictionary<string, Queue<GameObject>> poolDictionary;

	private void Awake(){
		Instance = this;
	}

	private void Start(){
		poolDictionary = new Dictionary<string, Queue<GameObject>>();
		foreach(Pool pool in pools){
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for(int i = 0; i < pool.size; i++){
				GameObject obj = Instantiate(pool.prefab);
				obj.name = pool.tag + " " + i;
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			if(!poolDictionary.ContainsKey(pool.tag)){
				poolDictionary.Add(pool.tag, objectPool);
			}
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation){
		GameObject obj = null;
		if(poolDictionary.ContainsKey(tag)){
			obj = poolDictionary[tag].Dequeue();
			obj.SetActive(true);
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			poolDictionary[tag].Enqueue(obj);
		} else{
			print("Pool with tag " + tag + " does not exist.");
		}
		return obj;
	}

	[System.Serializable]
	public class Pool{
		public string tag;
		public GameObject prefab;
		public int size;
	}
}
