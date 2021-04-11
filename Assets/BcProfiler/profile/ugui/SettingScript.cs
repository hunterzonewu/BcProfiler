using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour 
{
    public FpsProfiler fp = null;
    public Text beginTime = null;
    public Text profileTime = null;
    public Text cpuLoopCount = null;
    public Text gpuQuadCount = null;
    public Toggle onlyGetCpuGpuBound = null;

    void Start()
    {
        beginTime.text = FpsProfilerSetting.startTime.ToString();
        profileTime.text = (FpsProfilerSetting.endTime - FpsProfilerSetting.startTime).ToString();
        cpuLoopCount.text = FpsProfilerSetting.cpuProcCount.ToString();
        gpuQuadCount.text = FpsProfilerSetting.gpuQuadCount.ToString();
    }

    public void reset()
    { }

    public void set()
    {
        float.TryParse(beginTime.text, out FpsProfilerSetting.startTime);
        float profileTimeVal = 0f;
        bool bSuc = float.TryParse(profileTime.text, out profileTimeVal);
        FpsProfilerSetting.endTime = FpsProfilerSetting.startTime + profileTimeVal;
        long.TryParse(cpuLoopCount.text, out FpsProfilerSetting.cpuProcCount);
        int.TryParse(gpuQuadCount.text, out FpsProfilerSetting.gpuQuadCount);
        FpsProfilerSetting.bOnlyGetGpuCpuNeck = onlyGetCpuGpuBound.isOn;

        fp.recreateGpuChildNodes();
    }
}
