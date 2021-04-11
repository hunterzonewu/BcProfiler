using UnityEngine;
using System.Collections;

public abstract class FpsCase{

    GameObject m_tmpObj = null;
    string m_str = "";
    public GameObject tmpObj
    {
        get
        {
            if (null == m_tmpObj)
                m_tmpObj = GameObject.Find(m_str);
            return m_tmpObj;
        }
    }

    public FpsCase(string str)
    {
        m_str= str;
    }

	public virtual void show (bool bShow)
    {
        tmpObj.SetActive(bShow);
    }

    public static Renderer[] rs = null;
    public void show(string shaderName, bool bShow)
    {
        foreach (Renderer r in rs)
        {
            if (r.sharedMaterial.shader.name.Contains(shaderName)
                && !r.name.Contains("haishui")
                && !r.name.Contains("langhua")
                && !r.name.Contains("sky"))
                r.enabled = bShow;
        }
    }

}
