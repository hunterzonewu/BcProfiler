using UnityEngine;
using System.Collections;

public class FpsProfilerSetting
{
    public static float startTime = 5f;
    public static float endTime = 10f;
    public static long cpuProcCount = 0;
    public static int gpuQuadCount = 0;
    public static bool bOnlyGetGpuCpuNeck = true;   //只分析gpu or cpu bound.

    #region 瓶颈模拟
    static bool m_bNoNeck = true;
    static bool m_bCpuNeck = false;
    static bool m_bGpuNeck = false;
    public static bool bNoNeck
    {
        get { return m_bNoNeck; }
        set
        {
            if (false == value)
                return;

            m_bNoNeck = value;
            m_bCpuNeck = m_bGpuNeck = !m_bNoNeck;
        }
    }
    public static bool bCpuNeck
    {
        get { return m_bCpuNeck; }
        set
        {
            m_bCpuNeck = value;
            if (true == value)
                m_bNoNeck = m_bGpuNeck = !m_bCpuNeck;
            else
            {
                m_bNoNeck = true;
                m_bGpuNeck = false;
            }
        }
    }
    public static bool bGpuNeck
    {
        get { return m_bGpuNeck; }
        set
        {
            m_bGpuNeck = value;
            if (true == value)
                m_bNoNeck = m_bCpuNeck = !m_bGpuNeck;
            else
            {
                m_bNoNeck = true;
                m_bCpuNeck = false;
            }
        }
    }
    #endregion
}
