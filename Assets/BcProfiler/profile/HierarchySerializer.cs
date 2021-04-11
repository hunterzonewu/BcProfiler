#if UNITY_EDITOR
using UnityEditor;

 #endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;


public class HierarchySerializer
{
    [Serializable]
    class DataStruct
    {
        public string[] goNames;
        public int[] hierarchy;
        public int[] hash;
    }

    [Serializable]
    class GameObjectId
    {
        public string path;
        public string compStr;
        public string[] compTypeArr;
        public int hash;
    }

    static BinaryFormatter formatter = new BinaryFormatter();
    static GameObject[] objects;

    public static byte[] GetComponent(byte[] bytes)
    {
        MemoryStream stream = new MemoryStream(bytes);
        GameObjectId goi = (GameObjectId)formatter.Deserialize(stream);
        if (null == goi)
        {
            Logger.LogError("can not convert to GameObjectId");
            return null;
        }

        GameObject go = HierarchySerializer.FindGameObject(goi.path, goi.hash);
        if (null == go)
        {
            Debug.LogError(string.Format("can not find gameobject: {0}:{1}", goi.hash, goi.compStr));
            return null;
        }

        string compStr = "";
        ComponentSimulator.encode(out compStr, go, goi.compTypeArr);

        MemoryStream outstream = new MemoryStream();
        GameObjectId outGoi = new GameObjectId();
        outGoi.hash = go.GetHashCode();
        outGoi.compStr = compStr;
        formatter.Serialize(outstream, outGoi);
        byte[] outBytes = outstream.ToArray();

        return outBytes;
    }

    public static void ShowComponent(byte[] bytes)
    {
        MemoryStream stream = new MemoryStream(bytes);
        GameObjectId go = (GameObjectId)formatter.Deserialize(stream);
        if (null == go)
        {
            Logger.LogError("can not convert to GameObjectId");
            return;
        }

        //Debug.Log(string.Format("{0}:{1}", go.hash, go.compStr));
        GameObjectActiver[] goas = GameObject.FindObjectsOfType<GameObjectActiver>();
        for (int i=0; i<goas.Length; ++i)
        {
            if (go.hash == goas[i].Code)
            {
                ComponentSimulator cs = goas[i].GetComponent<ComponentSimulator>();
                cs.compStr = go.compStr;
                break;
            }
        }
    }

    public static void SetComponent(byte[] bytes)
    {
        MemoryStream stream = new MemoryStream(bytes);
        GameObjectId goi = (GameObjectId)formatter.Deserialize(stream);
        if (null == goi)
        {
            Logger.LogError("can not convert to GameObjectId");
            return;
        }

        GameObject go = HierarchySerializer.FindGameObject(goi.path, goi.hash);
        if (null == go)
        {
            Debug.LogError(string.Format("can not find gameobject: {0}:{1}", goi.hash, goi.compStr));
            return;
        } 
        ComponentSimulator.setComp(go, goi.compStr);

        //将修改后的值返回给服务端
        List<string> compTypeList = ComponentSimulator.getCompTypeList(goi.compStr);
        if (null != compTypeList)
        {
            string compStr = "";
            ComponentSimulator.encode(out compStr, go, compTypeList.ToArray());

            MemoryStream outstream = new MemoryStream();
            GameObjectId outGoi = new GameObjectId();
            outGoi.hash = go.GetHashCode();
            outGoi.compStr = compStr;
            formatter.Serialize(outstream, outGoi);
            byte[] outBytes = outstream.ToArray();
            RemoteGameObjectControl.RemoteSendComponent(outBytes);
        }
    }

    public static byte[] Serialize()
    {
        List<GameObject> list = new List<GameObject>();
        List<string> names = new List<string>();
        List<int> parents = new List<int>();
        List<int> hash = new List<int>();

        GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject pObject in pAllObjects)
        {
            if (pObject.transform.parent != null)
            {
                continue;
            }  

            if (pObject.hideFlags == HideFlags.NotEditable || pObject.hideFlags == HideFlags.HideAndDontSave)
            {
                continue;
            }
            
#if UNITY_EDITOR
            string sAssetPath = AssetDatabase.GetAssetPath(pObject.transform.root.gameObject);
            if (!string.IsNullOrEmpty(sAssetPath))
            {
                continue;
            }
         

 #endif
            list.Add(pObject);
            names.Add(pObject.name);
            hash.Add(pObject.GetHashCode());

            // highest bit used as active flag
            if(pObject.activeSelf)
            {
                parents.Add(0xffff);
            }
            else
            {
                parents.Add(0x7fff);
            }
        }

        int index = 0;
        while (index < list.Count)
        {
            GameObject go = list[index];
            for (int i = 0; i < go.transform.childCount; i++)
            {
                GameObject child = go.transform.GetChild(i).gameObject;
                list.Add(child);
                names.Add(child.name);
                hash.Add(child.GetHashCode());
                if(child.activeSelf)
                {
                    parents.Add(0x8000 | index);
                }
                else
                {
                    parents.Add(index);
                }
            }

            index++;
        }

        DataStruct data = new DataStruct();
        data.goNames = names.ToArray();
        data.hierarchy = parents.ToArray();
        data.hash = hash.ToArray();

        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, data);
        byte[] bytes = stream.ToArray();
        
        return bytes;
    }

    public static void Deserialize(byte[] bytes)
    {
        if (objects != null)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject.Destroy(objects[i]);
            }
            objects = null;
        }

        MemoryStream stream = new MemoryStream(bytes);
        DataStruct data = (DataStruct)formatter.Deserialize(stream);

        objects = new GameObject[data.goNames.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i] = new GameObject();
            objects[i].AddComponent<GameObjectActiver>().Code = data.hash[i];
            objects[i].AddComponent<ComponentSimulator>();

            objects[i].name = data.goNames[i];
            int parent = data.hierarchy[i] & 0x7fff;
            if (parent != 0x7fff)
            {
                objects[i].transform.parent = objects[parent].transform;
            }

            Debug.LogError(string.Format("[des] {0}:{1}", data.goNames[i], data.hash[i]));
        }
        
        for (int i = 0; i < objects.Length; i++)
        {
            if((data.hierarchy[i] & 0x8000) == 0)
            {
                objects[i].SetActive(false);
            }
        }

        Debug.LogFormat("deserialized objects count {0}", data.goNames.Length); 
    }

    static void BuildGameObjectPath(StringBuilder sb, Transform go)
    {
        if (go == null)
        {
            return;
        }

        BuildGameObjectPath(sb, go.parent);
        sb.Append('/');
        sb.Append(go.name);
    }

    static public byte[] BuildGameObjectId(GameObject go, string compStr = "", List<string> compTypeList = null)
    {
        StringBuilder sb = new StringBuilder();
        BuildGameObjectPath(sb, go.transform);
        string path = sb.ToString();

        GameObjectId id = new GameObjectId();
        id.path = path;
        if (!string.IsNullOrEmpty(compStr))
            id.compStr = compStr;
        if (null != compTypeList)
            id.compTypeArr = compTypeList.ToArray();

        if (go.GetComponent<GameObjectActiver>() != null)
        {
            id.hash = go.GetComponent<GameObjectActiver>().Code;        
        }
        else
        {
            Logger.LogError("BuildGameObjectId can not find component {0}", path);
        }

        Logger.LogError(string.Format("[Build] {0}:{1}", id.path, id.hash));
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, id);
        byte[] bytes = stream.ToArray();
        return bytes;
    }

    static public byte[] BuildMultiGameObjectId(GameObject[] gos, bool bActive)
    {
        if (null == gos)
            return null;

        List<GameObjectId> idList = new List<GameObjectId>();
        for (int i=0; i<gos.Length; ++i)
        {
            GameObject go = gos[i];
            StringBuilder sb = new StringBuilder();
            BuildGameObjectPath(sb, go.transform);
            string path = sb.ToString();

            GameObjectId id = new GameObjectId();
            id.path = path;
            if (go.GetComponent<GameObjectActiver>() != null)
            {
                go.GetComponent<GameObjectActiver>().setActive(bActive);
                id.hash = go.GetComponent<GameObjectActiver>().Code;
                idList.Add(id);
            }
            else
            {
                Logger.LogError("BuildGameObjectId can not find component {0}", path);
            }
        }
        Logger.Log("[BuildMultiGameObjectId] num of GameObject: {0}", idList.Count);
        if (0 == idList.Count)
            return null;

        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, idList.ToArray());
        byte[] bytes = stream.ToArray();
        return bytes;
    }

    static GameObject MathGameObject(string[] path, int index, GameObject go, int hash)
    {
        if(!path[index].Equals(go.name))
        {
            return null;
        }

        if(index == path.Length - 1)
        {
            if(go.GetHashCode() == hash)
            {
                return go;
            }
            else
            {
                return null;
            }
        }

        for(int i = 0; i < go.transform.childCount; i++)
        {
            GameObject match = MathGameObject(path, index + 1, go.transform.GetChild(i).gameObject, hash);
            if(match != null)
            {
                return match;
            }
        }

        return null;
    }


    static GameObject FindGameObject(string path, int hash)
    {
        string[] names = path.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
        
        GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject pObject in pAllObjects)
        {
            if (pObject.transform.parent != null)
            {
                continue;
            }

            if (pObject.hideFlags == HideFlags.NotEditable || pObject.hideFlags == HideFlags.HideAndDontSave)
            {
                continue;
            }

            GameObject go = MathGameObject(names, 0, pObject, hash);
            if (go != null)
            {
                return go;
            }
        }

        return null;
    }

    public static GameObject FindGameObject(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        GameObjectId id = (GameObjectId)formatter.Deserialize(stream);

        GameObject go = FindGameObject(id.path, id.hash);
        if (go == null)
        {
            Logger.LogError("can not find game object {0}", id.path);
        }
        return go;
    }

    public static GameObject[] FindMultiGameObject(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        GameObjectId[] ids = (GameObjectId[])formatter.Deserialize(stream);
        if (null == ids)
            return null;

        List<GameObject> goList = new List<GameObject>();
        for (int i=0; i<ids.Length; ++i)
        {
            GameObjectId id = ids[i];
            GameObject go = FindGameObject(id.path, id.hash);
            if (null == go)
            {
                Logger.LogError("can not find game object {0}", id.path);
            }
            else
            {
                //Debug.Log("GameObject: "+go, go);
                goList.Add(go);
            }
        }

        return goList.ToArray();
    }
}
