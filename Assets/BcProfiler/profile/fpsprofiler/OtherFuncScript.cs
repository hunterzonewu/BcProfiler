using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OtherFuncScript : MonoBehaviour 
{
    public Toggle noNeckToggle = null;
    public Toggle cpuNeckToggle = null;
    public Toggle gpuNeckToggle = null;
    public bool bIgnoreChange = false;

    public void OnValueChange(Toggle toggle)
    {
        if (null == toggle || bIgnoreChange)
            return;

        if (noNeckToggle == toggle)
        {
            bIgnoreChange = true;
            FpsProfilerSetting.bNoNeck = toggle.isOn;
            noNeckToggle.isOn = FpsProfilerSetting.bNoNeck;
            cpuNeckToggle.isOn = FpsProfilerSetting.bCpuNeck;
            gpuNeckToggle.isOn = FpsProfilerSetting.bGpuNeck;
        }
        else if (cpuNeckToggle == toggle)
        {
            bIgnoreChange = true;
            FpsProfilerSetting.bCpuNeck = toggle.isOn;
            noNeckToggle.isOn = FpsProfilerSetting.bNoNeck;
            gpuNeckToggle.isOn = FpsProfilerSetting.bGpuNeck;
        }
        else if (gpuNeckToggle == toggle)
        {
            bIgnoreChange = true;
            FpsProfilerSetting.bGpuNeck = toggle.isOn;
            noNeckToggle.isOn = FpsProfilerSetting.bNoNeck;
            cpuNeckToggle.isOn = FpsProfilerSetting.bCpuNeck;
        }
        bIgnoreChange = false;
    }

    public void click1b()
    {
        QualitySettings.blendWeights = BlendWeights.OneBone;
    }
    public void click2b()
    {
        QualitySettings.blendWeights = BlendWeights.TwoBones;
    }
    public void click4b()
    {
        QualitySettings.blendWeights = BlendWeights.FourBones;
    }
}
