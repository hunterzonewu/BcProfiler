using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

public class Logger
{
    public static void Log(string fmt, params System.Object[] values)
    {
        //Console.WriteLine(fmt, values);
        Debug.LogFormat(fmt, values);
    }


    public static void LogError(string fmt, params System.Object[] values)
    {
        //Console.WriteLine(fmt, values);
        Debug.LogErrorFormat(fmt, values);
    }
}

public class RemoteGameObjectControl : MonoBehaviour {
    
    public string HostAddress = "127.0.0.1";
    
    public enum Type
    {
        Server,
        Client
    }

    public Type type = Type.Server;

    static SocketMessager messager = new SocketMessager();

	// Use this for initialization
	void Start () {

        if(type == Type.Server)
        {
            messager.StartListen(HostAddress);
        }

        if(type == Type.Client)
        {
            messager.Connect(HostAddress);
        }	
	}
	
	// Update is called once per frame
	void Update () {

        SocketMessager.Msg[] msgs = messager.PopMessage();
        for(int i = 0; i < msgs.Length; i++)
        {
            if (msgs[i].cmd.Equals("upd"))
            {
                byte[] data = HierarchySerializer.Serialize();
                messager.Send("hie", data);
            }

            if(msgs[i].cmd.Equals("hie"))
            {
                HierarchySerializer.Deserialize(msgs[i].data);
            }
            
            if (msgs[i].cmd.Equals("act"))
            {
                GameObject go = HierarchySerializer.FindGameObject(msgs[i].data);
                if(go == null)
                {
                    //Logger.LogError("can not find game object {0}", path);
                }
                else
                {
                    go.SetActive(true);
                }
            }

            if (msgs[i].cmd.Equals("dac"))
            {
                GameObject go = HierarchySerializer.FindGameObject(msgs[i].data);
                if (go == null)
                {
                    //Logger.LogError("can not find game object");
                }
                else
                {
                    go.SetActive(false);
                }
            }

            if (msgs[i].cmd.Equals("mact"))
            {
                GameObject[] gos = HierarchySerializer.FindMultiGameObject(msgs[i].data);
                if (gos == null)
                {
                    //Logger.LogError("can not find game object {0}", path);
                }
                else
                {
                    for (int ii = 0; ii < gos.Length; ++ii)
                    {
                        GameObject go = gos[ii];
                        if (null != go)
                            go.SetActive(true);
                    }
                }
            }

            if (msgs[i].cmd.Equals("mdac"))
            {
                Logger.LogError("mdac");
                GameObject[] gos = HierarchySerializer.FindMultiGameObject(msgs[i].data);
                if (gos == null)
                {
                    Logger.LogError("mdac can not find game object");
                }
                else
                {
                    //Logger.LogError("mdac num of gameObject: "+gos.Length);
                    for (int ii = 0; ii < gos.Length; ++ii)
                    {
                        GameObject go = gos[ii];
                        if (null != go)
                            go.SetActive(false);
                    }
                }
            }

            if (msgs[i].cmd.Equals("get"))
            {
                //Debug.Log("wwq: getComponent");
                byte[] data = HierarchySerializer.GetComponent(msgs[i].data);
                messager.Send("com", data);
            }

            if (msgs[i].cmd.Equals("com"))
            {
                //Debug.Log("wwq: showComponent");
                HierarchySerializer.ShowComponent(msgs[i].data);
            }

            if (msgs[i].cmd.Equals("set"))
            {
                //Debug.Log("wwq: setComponent");
                HierarchySerializer.SetComponent(msgs[i].data);
            }
        }	
	}

    public static void RemoteActive(GameObject go, bool active)
    {
        byte[] data = HierarchySerializer.BuildGameObjectId(go);
        if(active)
        {
            messager.Send("act", data);
        }
        else
        {
            messager.Send("dac", data);
        }
    }

    public static void RemoteMultiActive(GameObject[] gos, bool active)
    {
        byte[] data = HierarchySerializer.BuildMultiGameObjectId(gos, active);
        if (active)
        {
            messager.Send("mact", data);
        }
        else
        {
            messager.Send("mdac", data);
        }
    }

    public static void RemoteSetComponent(GameObject go, string compStr)
    {
        byte[] data = HierarchySerializer.BuildGameObjectId(go, compStr);
        messager.Send("set", data);
    }

    public static void RemoteGetComponent(GameObject go, List<string> compTypeList)
    {
        byte[] data = HierarchySerializer.BuildGameObjectId(go, "", compTypeList);
        messager.Send("get", data);
    }

    public static void RemoteSendComponent(byte[] data)
    {
        messager.Send("com", data);
    }

    void OnGUI()
    {
        if(type == Type.Client)
        {
            return;
        }

        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(60), GUILayout.Width(120) };
        GUILayout.BeginVertical();

        if(messager.IsConnected)
        {
            GUILayout.Label(string.Format("client connected : {0}", messager.ConnectionInfo));
        }
        else
        {
            GUILayout.Label("client not connected");
        }

        if (GUILayout.Button("update", options))
        {
            messager.Send("upd");
        }

#if UNITY_EDITOR
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("active(多选)", options))
        {
            RemoteMultiActive(UnityEditor.Selection.gameObjects, true);
        }

        if (GUILayout.Button("deactive(多选)", options))
        {
            RemoteMultiActive(UnityEditor.Selection.gameObjects, false);
        }
#endif

        GUILayout.EndVertical();
    }

    void OnDestroy()
    {
        Logger.Log("socket destroy");
        messager.Dispose();
        messager = null;

        GC.Collect();
    }

}
