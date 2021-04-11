using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ComponentSimulator : MonoBehaviour
{
    public Component comp = null;
    // List<compTyoe, List<propName, <propType, <canWrite, propValue>>>>
    public List<KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>> propList =
        new List<KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>>();
    static List<string> propTypeList = new List<string>(new string[]{ "System.String", "System.Single", "System.Boolean"
                                                                  , "System.Int32", "System.UInt32"
        , "UnityEngine.Color", "UnityEngine.Vector3", "UnityEngine.Vector2", "UnityEngine.Vector4", "UnityEngine.Rect"
                                                                    , "UnityEngine.Mesh", "UnityEngine.Material", "UnityEngine.Texture2D"});
    protected string _compStr = "";
    public string compStr
    {
        get { return _compStr; }
        set
        {
            _compStr = value;
            decode(ref propList, value);
        }
    }

    public int Code
    {
        get;
        set;
    }

    public static void decode(ref List<KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>> propList, string compStr)
    {
        if (string.IsNullOrEmpty(compStr))
            return;

        propList = new List<KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>>();
        string[] compArr = compStr.Split(new string[] { "_cawwq_" }, StringSplitOptions.None);
        for (int i = 0; i < compArr.Length; ++i)
        {
            if (string.IsNullOrEmpty(compArr[i]))
                continue;

            string[] paramArr = compArr[i].Split(new string[] { "_pawwq_" }, StringSplitOptions.None);
            //Debug.Log("component: " + paramArr[0]);
            List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>> compPropList = new List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>();
            for (int j = 1; j < paramArr.Length; ++j)
            {
                if (string.IsNullOrEmpty(paramArr[j]))
                    continue;

                string[] paras = paramArr[j].Split(new string[] { "_pwwq_" }, StringSplitOptions.None);
                if (4 != paras.Length)
                {
                    Debug.LogError(paramArr[j]);
                    continue;
                }

                //Debug.Log(paras[0] + " " + paras[1] + " " + paras[2] + " " + paras[3]);
                compPropList.Add(new KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>(paras[0], new KeyValuePair<string, KeyValuePair<string, string>>(paras[1], new KeyValuePair<string, string>(paras[2], paras[3]))));
            }
            if (0 < compPropList.Count)
                propList.Add(new KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>(paramArr[0], compPropList));
        }
    }

    public static void encode(out string compStr
                              , List<KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>> propList
                              , List<KeyValuePair<string, List<string>>> propValList)
    {
        compStr = "";
        if (propList.Count != propValList.Count)
            return;

        for (int i = 0; i < propList.Count; ++i)
        {
            compStr += string.Format("_cawwq_{0}", propList[i].Key);
            for (int j = 0; j < propList[i].Value.Count; ++j)
            {
                string str = string.Format("{0}_pwwq_{1}_pwwq_{2}_pwwq_{3}", propList[i].Value[j].Key, propList[i].Value[j].Value.Key, propList[i].Value[j].Value.Value.Key, propValList[i].Value[j]);
                compStr += "_pawwq_" + str;
            }
        }
        //Debug.Log(compStr);
    }

    public static void encode(out string compStr, GameObject go, string[] compTypeArr)
    {
        compStr = "";
        if (null == compTypeArr || 0 == compTypeArr.Length)
            return;

        for (int i = 0; i < compTypeArr.Length; ++i)
        {
            Type type = Type.GetType(compTypeArr[i]);
            if (null == type)
            {
                Debug.LogError("get type error! " + compTypeArr[i]);
                continue;
            }

            Component component = go.GetComponent(type);
            if (null == component)
            {
                Debug.LogError("getComponent error! " + type);
                continue;
            }

            string subCompStr = "";
            encode(out subCompStr, component);
            compStr += subCompStr;
        }

        //Debug.Log("compStr: " + compStr);
    }

    /**
     * 输出格式：{(compName)_[pawwq_(propName)_pwwq_(propType)_pwwq_{CanWrite}_pwwq_(propValue)]_[pawwq_(propName2)_pwwq_(propType2)_pwwq_{CanWrite}_pwwq_(propValue2)]_...}_cawwq_{(compName2)_...}_cawwq_...
     */
    //[IFix.Patch]
    public static void encode(out string compStr, Component comp)
    {
        compStr = "";
        if (null == comp)
            return;

        try
        {
            Type type = comp.GetType();
            if (null == type)
            {
                return;
            }
            PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            compStr += string.Format("_cawwq_{0}", type.AssemblyQualifiedName);
            //Debug.Log("count of pi: " + pis.Length);
            for (int j = 0; j < pis.Length; ++j)
            {
                //Debug.Log(string.Format("prop type: {0}, canWrite: {1}, canRead: {2}", pis[j].Name, pis[j].CanWrite, pis[j].CanRead));

                string propType = pis[j].PropertyType.FullName;
                if (!propTypeList.Contains(propType))
                {
                    Debug.LogError("not matched prop type: " + propType);
                    continue;
                }
                //不访问material，防止材质球进行实例化
                if ("material" == pis[j].Name)
                {
                    continue;
                }

                MethodInfo mi = pis[j].GetGetMethod();
                object val = mi.Invoke(comp, null);
                string valStr = TypeOpe.TypeToString(propType, val);
                string str = string.Format("{0}_pwwq_{1}_pwwq_{2}_pwwq_{3}", pis[j].Name, propType, pis[j].CanWrite, valStr);
                compStr += "_pawwq_" + str;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("encode error: " + e.ToString());
        }
        //Debug.Log("wwq: " + compStr);
    }

    public static void setComp(GameObject go, string compStr)
    {
        if (null == go || string.IsNullOrEmpty(compStr))
            return;

        List<KeyValuePair<string, List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>>>> propList = null;
        ComponentSimulator.decode(ref propList, compStr);

        for (int i = 0; i < propList.Count; ++i)
        {
            Type type = Type.GetType(propList[i].Key);
            if (null == type)
            {
                Debug.LogError("getType error! " + propList[i].Key);
                continue;
            }

            Component comp = go.GetComponent(type);
            if (null == comp)
            {
                Debug.LogError("getComponent error! " + go + " " + propList[i].Key);
                continue;
            }

            List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>> compPropList = propList[i].Value;
            PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int j = 0; j < pis.Length; ++j)
            {
                if (!pis[j].CanWrite)
                    continue;

                int idx = compPropList.FindIndex(p => p.Key == pis[j].Name);
                if (-1 == idx)
                    continue;

                MethodInfo mi = pis[j].GetSetMethod();
                Type propType = pis[j].PropertyType;
                string val = compPropList[idx].Value.Value.Value;
                object obj = TypeOpe.StringToType(propType, val);
                if (null == obj)
                    continue;

                object[] valParam = new object[] { obj };
                try
                {
                    mi.Invoke(comp, valParam);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }

    public static List<string> getCompTypeList(string compStr)
    {
        if (string.IsNullOrEmpty(compStr))
            return null;

        List<string> typeList = new List<string>();
        string[] compArr = compStr.Split(new string[] { "_cawwq_" }, StringSplitOptions.None);
        for (int i = 0; i < compArr.Length; ++i)
        {
            if (string.IsNullOrEmpty(compArr[i]))
                continue;

            string[] paramArr = compArr[i].Split(new string[] { "_pawwq_" }, StringSplitOptions.None);
            typeList.Add(paramArr[0]);
        }

        return typeList;
    }
}
