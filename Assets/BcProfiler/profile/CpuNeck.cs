using UnityEngine;
using System.Collections;

public class CpuNeck : MonoBehaviour
{

    float m_f = 1.3745f;
    public static float time_ = 0f;

    private void OnDisable()
    {
        time_ = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float preTime = Time.realtimeSinceStartup;
        long count = FpsProfilerSetting.cpuProcCount * 10000;
        for (long i = 0; i < count; ++i)
        {
            m_f *= i;
        }

        m_f = 1.3745f;
        time_ = (Time.realtimeSinceStartup - preTime) * 1000;
    }
}
