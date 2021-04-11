using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OtherSettingScript : MonoBehaviour
{
    public Text statusTex = null;
    public Text statusTex2 = null;
    public Text neckTex = null;
    public Text neckTex2 = null;
    public FpsProfiler profiler = null;
    public void OnClick()
    {
        if (FpsProfilerSetting.bNoNeck)
        {
            statusTex.text = "";
            statusTex2.text = "";
            profiler.m_cpuNeck.SetActive(false);
            profiler.m_gpuNeck.SetActive(false);
            neckTex.gameObject.SetActive(false);
            neckTex2.gameObject.SetActive(false);
        }
        else if (FpsProfilerSetting.bCpuNeck)
        {
            statusTex.text = "正在模拟cpu瓶颈";
            statusTex2.text = "正在模拟cpu瓶颈";
            profiler.m_cpuNeck.SetActive(true);
            profiler.m_gpuNeck.SetActive(false);
            neckTex.gameObject.SetActive(true);
            neckTex2.gameObject.SetActive(true);
        }
        else if (FpsProfilerSetting.bGpuNeck)
        {
            statusTex.text = "正在模拟gpu瓶颈";
            statusTex2.text = "正在模拟gpu瓶颈";
            profiler.m_cpuNeck.SetActive(false);
            profiler.m_gpuNeck.SetActive(true);
            neckTex.gameObject.SetActive(true);
            neckTex2.gameObject.SetActive(true);
        }
    }
}
