using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

[CustomEditor(typeof(ComponentSimulator))]
public class ComponentSimulatorEditor : Editor
{
    ComponentSimulator cs = null;
    List<KeyValuePair<string, List<string>>> propValList = new List<KeyValuePair<string, List<string>>>();
    static List<string> notSupportEditTypeList = new List<string>(new string[] { "UnityEngine.Mesh", "UnityEngine.Material", "UnityEngine.Texture2D" });
    private void OnEnable()
    {
        cs = target as ComponentSimulator;
        SyncPropVal();
    }

    string extraCompName = "";
    string finalCompName = "";
    string preExtra = "";

    static string getQualifiedName(string className)
    {
        string qualifiedName = "";
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.Name == className)
                {
                    qualifiedName = type.AssemblyQualifiedName;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(qualifiedName))
                break;
        }

        return qualifiedName;
    }

    static List<string> m_defaultCompTypeList = null;
    static List<string> defaultCompTypeList
    {
        get
        {
            if (null == m_defaultCompTypeList)
            {
                m_defaultCompTypeList = new List<string>();
                string psComp = getQualifiedName("ParticleSystem");
                m_defaultCompTypeList.Add(psComp);
            }
            return m_defaultCompTypeList;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.ObjectField("组件", cs.comp, typeof(UnityEngine.Object));

        if (cs.propList.Count != propValList.Count)
            SyncPropVal();
        else
        {
            for (int i = 0; i < cs.propList.Count; ++i)
            {
                if (cs.propList[i].Value.Count != propValList[i].Value.Count)
                {
                    SyncPropVal();
                    break;
                }
            }
        }
        EditorGUILayout.BeginVertical("helpbox");
        extraCompName = EditorGUILayout.TextField("额外组件类型(类名)", extraCompName);
        EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(finalCompName)?"没有此类型！":finalCompName);
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("GetComponents"))
        {
            if (preExtra != extraCompName||string.IsNullOrEmpty(finalCompName))
            {
                finalCompName = getQualifiedName(extraCompName);
            }
            preExtra = extraCompName;
            List<string> compTypeList = new List<string>(defaultCompTypeList);
            if (!string.IsNullOrEmpty(finalCompName) && !compTypeList.Contains(finalCompName))
                compTypeList.Add(finalCompName);
            RemoteGameObjectControl.RemoteGetComponent(cs.gameObject, compTypeList);
        }
        for (int i=0; i<cs.propList.Count; ++i)
        {
            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.LabelField(cs.propList[i].Key);
            for (int j = 0; j < cs.propList[i].Value.Count; ++j)
            {
                KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>> kvp = cs.propList[i].Value[j];
                bool bCanWrite = false;
                bool.TryParse(kvp.Value.Value.Key, out bCanWrite);
                string propType = kvp.Value.Key;
                if (notSupportEditTypeList.Contains(propType))
                    bCanWrite = false;
                EditorGUI.BeginDisabledGroup(!bCanWrite);
                string val = propValList[i].Value[j];
                if ("UnityEngine.Color" == propType)
                {
                    Color cVal = Color.clear;
                    if (TypeOpe.ColorTryParse(val, out cVal))
                    {
                        cVal = EditorGUILayout.ColorField(kvp.Key, cVal);
                        propValList[i].Value[j] = TypeOpe.TypeToString(propType, (object)cVal);
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("UnityEngine.Vector2" == propType)
                {
                    Vector2 vVal = Vector2.zero;
                    if (TypeOpe.Vector2TryParse(val, out vVal))
                    {
                        vVal = EditorGUILayout.Vector2Field(kvp.Key, vVal);
                        propValList[i].Value[j] = TypeOpe.TypeToString(propType, (object)vVal);
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("UnityEngine.Vector3" == propType)
                {
                    Vector3 vVal = Vector3.zero;
                    if (TypeOpe.Vector3TryParse(val, out vVal))
                    {
                        vVal = EditorGUILayout.Vector3Field(kvp.Key, vVal);
                        propValList[i].Value[j] = TypeOpe.TypeToString(propType, (object)vVal);
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("UnityEngine.Vector4" == propType)
                {
                    Vector4 vVal = Vector4.zero;
                    if (TypeOpe.Vector4TryParse(val, out vVal))
                    {
                        vVal = EditorGUILayout.Vector4Field(kvp.Key, vVal);
                        propValList[i].Value[j] = TypeOpe.TypeToString(propType, (object)vVal);
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("UnityEngine.Rect" == propType)
                {
                    Rect vVal = new Rect(0f,0f,0f,0f);
                    if (TypeOpe.RectTryParse(val, out vVal))
                    {
                        vVal = EditorGUILayout.RectField(kvp.Key, vVal);
                        propValList[i].Value[j] = TypeOpe.TypeToString(propType, (object)vVal);
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("System.Boolean" == propType)
                {
                    System.Boolean bVal = false;
                    if (System.Boolean.TryParse(val, out bVal))
                    {
                        bVal = EditorGUILayout.Toggle(kvp.Key, bVal);
                        propValList[i].Value[j] = bVal.ToString();
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("System.Single" == propType)
                {
                    System.Single fVal = 0f;
                    if (System.Single.TryParse(val, out fVal))
                    {
                        fVal = EditorGUILayout.FloatField(kvp.Key, fVal);
                        propValList[i].Value[j] = fVal.ToString();
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else if ("System.Int32" == propType || "System.UInt32" == propType)
                {
                    System.Int32 iVal = 0;
                    if (System.Int32.TryParse(val, out iVal))
                    {
                        iVal = EditorGUILayout.IntField(kvp.Key, iVal);
                        propValList[i].Value[j] = iVal.ToString();
                    }
                    else
                    {
                        propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                    }
                }
                else
                {
                    propValList[i].Value[j] = EditorGUILayout.TextField(kvp.Key + "(" + kvp.Value.Key + ")", val);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("submit"))
        {
            string compStr = "";
            ComponentSimulator.encode(out compStr, cs.propList, propValList);
            RemoteGameObjectControl.RemoteSetComponent(cs.gameObject, compStr);
        }
        /*
        if (GUILayout.Button("submit0"))
        {
            string compStr = "";
            ComponentSimulator.encode(out compStr, cs.comp.name, cs.propList, propValList);
            List<KeyValuePair<string, KeyValuePair<string, string>>> propList = null;
            ComponentSimulator.decode(ref propList, compStr);

            Type type = Type.GetType("UnityEngine.ParticleSystem, UnityEngine");
            if (null == type)
            {
                return;
            }
            PropertyInfo[] pis = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int j = 0; j < pis.Length; ++j)
            {
                if (!pis[j].CanWrite || !pis[j].CanRead)
                    continue;

                int idx = propList.FindIndex(p => p.Key == pis[j].Name);
                if (-1 == idx)
                    continue;

                MethodInfo mi = pis[j].GetSetMethod();
                Type propType = pis[j].PropertyType;
                object[] val = null;
                if (typeof(System.Single) == propType)
                {
                    System.Single tmp;
                    if (System.Single.TryParse(propList[idx].Value.Value, out tmp))
                        val = new object[] { tmp };
                    else
                        continue;
                }
                else if (typeof(System.Boolean) == propType)
                {
                    System.Boolean tmp;
                    if (System.Boolean.TryParse(propList[idx].Value.Value, out tmp))
                        val = new object[] { tmp };
                    else
                        continue;
                }
                else if (typeof(System.Int32) == propType)
                {
                    System.Int32 tmp;
                    if (System.Int32.TryParse(propList[idx].Value.Value, out tmp))
                        val = new object[] { tmp };
                    else
                        continue;
                }
                else if (typeof(System.UInt32) == propType)
                {
                    System.UInt32 tmp;
                    if (System.UInt32.TryParse(propList[idx].Value.Value, out tmp))
                        val = new object[] { tmp };
                    else
                        continue;
                }
                else if (typeof(System.String) == propType)
                {
                    val = new object[] { propList[idx].Value.Value };
                }
                else
                    continue;

                try
                {
                    mi.Invoke(cs.comp, val);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        }
*/
    }

    void SyncPropVal()
    {
        propValList.Clear();
        for (int i = 0; i < cs.propList.Count; ++i)
        {
            List<KeyValuePair<string, KeyValuePair<string, KeyValuePair<string, string>>>> propTypeValList = cs.propList[i].Value;
            List<string> valList = new List<string>();
            for (int j = 0; j < propTypeValList.Count; ++j)
                valList.Add(propTypeValList[j].Value.Value.Value);
            propValList.Add(new KeyValuePair<string, List<string>>(cs.propList[i].Key, valList));
        }
    }
}
