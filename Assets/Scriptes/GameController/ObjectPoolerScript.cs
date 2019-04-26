using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class PooledGameObject
{
	public PooledGOType gameObjectType ;
	public GameObject prefab;
	public int pooledAmount = 5;
}


public class ObjectPoolerScript : MonoBehaviour {

	public static ObjectPoolerScript op;

//	public static objectPoolerScript current;
	Dictionary<PooledGOType,List<GameObject>> pooledObjectsDic = new Dictionary<PooledGOType, List<GameObject>>();
	Dictionary<PooledGOType,GameObject> objectPrefabDic = new Dictionary<PooledGOType, GameObject>();
	public PooledGameObject[] pooledGOs;

	// List<GameObject> pooledObjects;

	// void Awake (){
	// }

	// Use this for initialization
	void Awake () {
		op = this;
		foreach(PooledGameObject pooledGO in pooledGOs){
			objectPrefabDic.Add(pooledGO.gameObjectType,pooledGO.prefab);
			List<GameObject> pooledObjects = new List<GameObject>();
			for (int i = 0; i < pooledGO.pooledAmount; i++) {
				GameObject obj = (GameObject) Instantiate(pooledGO.prefab);
				obj.SetActive(false);
				pooledObjects.Add(obj);
			}
			pooledObjectsDic.Add(pooledGO.gameObjectType,pooledObjects);
		}
		
	}

	public GameObject GetPooledObject(PooledGOType gameObjectType){
		List<GameObject> pooledObjects = pooledObjectsDic[gameObjectType];
		for (int i = 0; i < pooledObjects.Count; i++) {
			if(!pooledObjects[i].activeInHierarchy){
				return pooledObjects[i];
			}
		}
		GameObject obj = (GameObject) Instantiate(objectPrefabDic[gameObjectType]);
		pooledObjects.Add(obj);
		Debug.Log("grew to: " + pooledObjects.Count);
		return obj;
	}
}
