using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FpsProfiler : MonoBehaviour
{
    #region profiler result
    public static float m_curGpuTime = 0f;
    public static float m_curCpuTime = 0f;
    public static FrameNeck curFrameNeck
    {
        get
        {
            if (m_curGpuTime > m_curCpuTime)
                return FrameNeck.GPU;
            else if (m_curGpuTime < m_curCpuTime)
                return FrameNeck.CPU;
            else
                return FrameNeck.TOTAL;
        }
    }

    public enum FrameNeck
    {
        CPU,
        GPU,
        TOTAL
    }
    #endregion

    #region callback
    public delegate void beginProfilerFunc();
    public delegate void endProfilerFunc();
    public static beginProfilerFunc beginProfilerCallback = null;
    public static endProfilerFunc endProfilerCallback = null;
    #endregion

    #region 瓶颈控制go
    public GameObject m_gpuNeck = null;
    public GameObject m_cpuNeck = null;
    public GameObject m_gpuQuadTemplate = null;
    #endregion

    [HideInInspector] public bool bWork = false;
	protected FpsCaseManager m_fpsCaseManager = null;
	protected IEnumerator m_profileCoroutine = null;
    protected float m_curConsumeTime = 0.0f;
    protected float m_curFps = 0.0f;
    protected static float m_gpuNeckTime = 0.0f;
    public static float gpuNeckTime { get { return m_gpuNeckTime; } }
    protected float m_cpuNeckTime = 0.0f;
    protected float curConsumeTime
    {
        get { return m_curConsumeTime; }
        set { m_curConsumeTime = value; }
    }

	protected FpsProfiler()
	{
		m_fpsCaseManager = new FpsCaseManager ();
	}

    void Start()
    {
        DontDestroyOnLoad(this);
        ////m_gpuNeck = GameObject.Find("gpuneck");
        ////m_cpuNeck = GameObject.Find("cpuneck");
        ////for (int i = 0; i < transform.childCount; ++i)
        //    ////LogWrapper.LogInfo("child: "+transform.GetChild(i));
        //m_gpuNeck = GameObject.Find("gpuneck");
        //m_cpuNeck = GameObject.Find("cpuneck");
        //m_gpuQuadTemplate = m_gpuNeck.transform.Find("quadTemplate").gameObject;
        //Debug.Log("gpu neck: " + m_gpuNeck);
        //Debug.Log("cpu neck: " + m_cpuNeck);
        //Debug.Log("gpu quad template: " + m_gpuQuadTemplate);
        m_gpuNeck.SetActive(false);
        m_cpuNeck.SetActive(false);
        m_gpuQuadTemplate.SetActive(false);
    }

    public void profile()
    {
        profile(null);
    }
	public void profile(Renderer[] rc)
    {
        //开始profiler
        if (null != beginProfilerCallback)
            beginProfilerCallback();
        //string path = System.IO.Path.Combine(Application.persistentDataPath, "fpsprofiler.txt");
        //try
        //{
        //    string[] strs = System.IO.File.ReadAllLines(path);
        //    targetFps = float.Parse(strs[0]);
        //    startTime = float.Parse(strs[1]);
        //    endTime = float.Parse(strs[2]);
        //    cpuProcCount = long.Parse(strs[3]);
        //    gpuQuadCount = int.Parse(strs[4]);
        //}
        //catch (System.Exception e)
        //{
        //    //LogWrapper.LogProf("err: "+e);
        //}
        //LogWrapper.LogProf("startTime: " + FpsProfilerSetting.startTime + ", endTime: " + FpsProfilerSetting.endTime + ", cpuProcCount: " + FpsProfilerSetting.cpuProcCount + ", gpuQuadCount: " + FpsProfilerSetting.gpuQuadCount);
        recreateGpuChildNodes();
        //LogWrapper.LogProf("FpsProfiler.profile");
		m_profileCoroutine = profileTimeConsumer (TimeConsumer.GPU);
        StartCoroutine(m_profileCoroutine);
	}

    public void recreateGpuChildNodes()
    {
        Transform gpuQuadParent = m_gpuQuadTemplate.transform.parent;
        //del child node
        for (int i = 0; i < gpuQuadParent.childCount; ++i)
        {
            GameObject tmpChild = gpuQuadParent.GetChild(i).gameObject;
            if (tmpChild != m_gpuQuadTemplate)
                GameObject.Destroy(tmpChild);
        }
        //create child node1
        GameObject node1 = new GameObject("1");
        node1.transform.parent = gpuQuadParent;
        m_gpuQuadTemplate.SetActive(true);
        for (int i = 0; i < FpsProfilerSetting.gpuQuadCount; ++i)
        {
            GameObject tmpQuad = GameObject.Instantiate(m_gpuQuadTemplate, node1.transform, true) as GameObject;
        }
        m_gpuQuadTemplate.SetActive(false);
        //dumplicate child node2,node3,node4
        for (int i = 2; i <= 4; ++i)
        {
            GameObject tmpNode = GameObject.Instantiate(node1, gpuQuadParent) as GameObject;
            tmpNode.name = i.ToString();
            if (i >= 3)
                tmpNode.SetActive(false);
        }
        //
        gpuQuadParent.FindChild("3").gameObject.SetActive(false);
        gpuQuadParent.FindChild("4").gameObject.SetActive(false);
    }

    public void init(Renderer[] rc)
    {
        //m_fpsCaseManager.init(rc);
    }

	public enum TimeConsumer
	{
		GPU,
		CPU
	}

    public void getGpuNeckTime()
    {
        if (FpsProfilerSetting.bGpuNeck)
        {
            StartCoroutine(calcGpuNeckTime());
        }
    }

    float fps = 0f;
    int count = 0;
    //StringBuilder sb = new StringBuilder(1024);
    bool getTime(float preTime, TimeConsumer tc)
    {
        float deltaTime = Time.unscaledTime-preTime;

        //if (TimeConsumer.GPU == tc)
        {
            if (deltaTime >= FpsProfilerSetting.startTime && m_curFps < EngineProfiler.FPSDisplay.fps_)
                m_curFps = EngineProfiler.FPSDisplay.fps_;

            if (deltaTime >= FpsProfilerSetting.endTime)
            {
                m_curConsumeTime = 1000 / m_curFps;
                return true;
            }
        }
        //else
        //{
        //    if (deltaTime >= startTime)
        //    {
        //        fps += EngineProfiler.FPSDisplay.fps_;
        //        count++;
        //    }

        //    if (deltaTime >= endTime)
        //    {
                
        //        m_curConsumeTime = 1000/(fps / count);
        //        fps = 0f;
        //        count = 0;
        //        return true;
        //    }
        //}

        return false;
    }

    IEnumerator getCurConsumeTime(TimeConsumer tc)
    {
        float preTime = Time.unscaledTime;
        m_curConsumeTime = 0f;
        m_curFps = 0;
        yield return new WaitUntil(()=>getTime(preTime, tc));
    }

    IEnumerator calcGpuNeckTime()
    {
        m_gpuNeck.SetActive(true);
        Transform gpuNeckChild = m_gpuNeck.transform.GetChild(0);
        for (int i = 0; i < gpuNeckChild.childCount; ++i)
            gpuNeckChild.GetChild(i).gameObject.SetActive(true);
        m_gpuQuadTemplate.SetActive(false);
        yield return getCurConsumeTime(TimeConsumer.GPU);
        float cosTime1 = m_curConsumeTime;
        //LogWrapper.LogInfo("cosTime1: "+cosTime1);
        gpuNeckChild.FindChild("3").gameObject.SetActive(false);
        gpuNeckChild.FindChild("4").gameObject.SetActive(false);
        yield return getCurConsumeTime(TimeConsumer.GPU);
        float cosTime2 = m_curConsumeTime;
        //LogWrapper.LogInfo("cosTime2: "+cosTime2);
        m_gpuNeckTime = cosTime1 - cosTime2;
        //LogWrapper.LogProf("gpu neck time: "+m_gpuNeckTime);
    }

    float cpuTime = 0;
    float cpuCount = 0;
    bool getCpuTime(float preTime)
    {
        float deltaTime = Time.unscaledTime - preTime;
        if (deltaTime >= FpsProfilerSetting.startTime)
        {
            cpuTime += CpuNeck.time_;
            cpuCount++;
        }

        if (deltaTime >= FpsProfilerSetting.endTime)
        {
            m_cpuNeckTime = cpuTime / cpuCount;
            cpuTime = 0;
            cpuCount = 0;
            return true;
        }

        return false;
    }

    IEnumerator calcCpuNeckTime()
    {
        m_cpuNeck.SetActive(true);
        float preTime = Time.unscaledTime;
        yield return new WaitUntil(() => getCpuTime(preTime));
        //LogWrapper.LogProf("cpu neck time: " + m_cpuNeckTime);
    }

    //float calcCpuNeckTime()
    //{
    //    return 15f;
    //}

    void showGpuNeck(bool bShow)
    {
        m_gpuNeck.SetActive(bShow);
    }

    void showCpuNeck(bool bShow)
    {
        m_cpuNeck.SetActive(bShow);
    }

    float getRealTime()
    {
        return 0.0f;
    }

    bool isTargetFps(float fps)
    {
        return fps <= EngineProfiler.FPSDisplay.fps_;
    }

    /*
     * 先隐藏所有，再通过显示个别来测试耗时不准。
     * step1.   显示所有东西
     * step2.   deactive cpu&gpu neck
     * step3.   if 计算gpu耗时
     * step3.1  计算gpu片的耗时
     * step3.2  active gpu neck，保证当前是gpu瓶颈，计算当前一帧耗时total
     * step3.3  隐藏某物体，计算当前一帧耗时a，则total-a为该物体的gpu耗时
     * step3.4  重复step3.3，计算出其他所有物体的gpu耗时
     * step3.5  总耗时total-所有物体的gpu总耗时-gpu片的耗时 = unity自身的耗时
     * step4.   else 计算cpu耗时
     * step4.1  计算cpu脚本的耗时，后面的步骤同上
     */
    protected IEnumerator profileTimeConsumer(TimeConsumer tc)
	{
        bWork = true;
        m_fpsCaseManager.showAll(true);
        if (null != m_cpuNeck)
            m_cpuNeck.SetActive(false);
        if (null != m_gpuNeck)
            m_gpuNeck.SetActive(false);

		float neckTime = 0.0f;
		string tcFlag = "";
		if (TimeConsumer.GPU == tc) 
        {
			tcFlag = "gpu";
			yield return calcGpuNeckTime ();
            neckTime = m_gpuNeckTime;
		} else 
        {
			tcFlag = "cpu";
            yield return calcCpuNeckTime();
            neckTime = m_cpuNeckTime;
		}
		//LogWrapper.LogProf (tcFlag+" consumer (ms):");
		string logStr = "显示"+tcFlag+"瓶颈:";
        if (TimeConsumer.GPU == tc)
            showGpuNeck(true);
        else
            showCpuNeck(true);
        yield return getCurConsumeTime(tc);
		float neckRefTime = curConsumeTime;
		logStr += "\t" + neckRefTime;
		//LogWrapper.LogProf (logStr);
        float totalTime = 0f;
        if (!FpsProfilerSetting.bOnlyGetGpuCpuNeck)
        {
            int iCount = m_fpsCaseManager.fpsCaseDic.Count;
            //LinkedListNode<H3DPair<FpsCaseManager.FPS_CASE, FpsCase>> fpsCaseNode = m_fpsCaseManager.fpsCaseDic.Begin();
            var fpsCaseNode = m_fpsCaseManager.fpsCaseDic.GetEnumerator();
            //for (; null != fpsCaseNode; fpsCaseNode = fpsCaseNode.Next)
            while (fpsCaseNode.MoveNext())
            {
                logStr = "隐藏" + fpsCaseNode.Current.Key + ":";
                if (FpsCaseManager.FPS_CASE.FC_AVATAR_OTHER == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_SHEQU_OTHER == fpsCaseNode.Current.Key)
                {
                    showOther(fpsCaseNode.Current.Key, false);
                }
                else
                    m_fpsCaseManager.show(fpsCaseNode.Current.Key, false);
                yield return getCurConsumeTime(tc);
                float curNeckTime = curConsumeTime;
                float tmpTime = neckRefTime - curNeckTime;
                if (tmpTime > 0)
                    totalTime += tmpTime;
                logStr += "\t" + curNeckTime + "\t" + tmpTime;
                //LogWrapper.LogProf(logStr);
                m_fpsCaseManager.show(fpsCaseNode.Current.Key, true);
            }
        }
        float unityTime = neckRefTime-totalTime-neckTime;
        if (unityTime < 0)
            unityTime = 0f;
        //LogWrapper.LogProf("unity:\t" + unityTime);
        //LogWrapper.LogProf("total:\t" + (unityTime + totalTime) + "ms\t预计帧率: " + (1000 / (unityTime + totalTime)));
        if (TimeConsumer.GPU == tc)
            m_curGpuTime = unityTime;
        else if (TimeConsumer.CPU == tc)
            m_curCpuTime = unityTime;

        m_fpsCaseManager.showAll(true);
        if (TimeConsumer.GPU == tc)
            showGpuNeck(false);
        else
            showCpuNeck(false);
        
        //搞完gpu耗时，需要继续测完cpu耗时
        if (TimeConsumer.GPU == tc)
        {
            m_profileCoroutine = profileTimeConsumer(TimeConsumer.CPU);
            StartCoroutine(m_profileCoroutine);
        }
        else
        {
            //都搞完了就结束
            bWork = false;

            if (null != endProfilerCallback)
                endProfilerCallback();
        }
	}

    public void show(FpsCaseManager.FPS_CASE fpsCase, bool bShow)
    {
        m_fpsCaseManager.show(fpsCase, bShow);
    }

    void showOther(FpsCaseManager.FPS_CASE fpsCase, bool bShow)
    {
        m_fpsCaseManager.show(fpsCase, bShow);
        if (FpsCaseManager.FPS_CASE.FC_AVATAR_OTHER == fpsCase)
        {
		    //LinkedListNode<H3DPair<FpsCaseManager.FPS_CASE, FpsCase>> fpsCaseNode = m_fpsCaseManager.fpsCaseDic.Begin ();
            var fpsCaseNode = m_fpsCaseManager.fpsCaseDic.GetEnumerator();
            //for (; null != fpsCaseNode; fpsCaseNode = fpsCaseNode.Next)
            while (fpsCaseNode.MoveNext())
            {
                if (FpsCaseManager.FPS_CASE.FC_AVATAR_PARTICLE == fpsCaseNode.Current.Key
                    ||FpsCaseManager.FPS_CASE.FC_AVATAR_BLEND == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_AVATAR_BP == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_AVATAR_LINKSKINMESH == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_AVATAR_NAMEPLATE == fpsCaseNode.Current.Key
                    )
                {
                    m_fpsCaseManager.show(fpsCaseNode.Current.Key, !bShow);
                }
            }
        }
        else
        {
            //LinkedListNode<H3DPair<FpsCaseManager.FPS_CASE, FpsCase>> fpsCaseNode = m_fpsCaseManager.fpsCaseDic.Begin();
            var fpsCaseNode = m_fpsCaseManager.fpsCaseDic.GetEnumerator();
            //for (; null != fpsCaseNode; fpsCaseNode = fpsCaseNode.Next)
            while (fpsCaseNode.MoveNext())
                //for (; null != fpsCaseNode; fpsCaseNode = fpsCaseNode.Next)
            {
                if (FpsCaseManager.FPS_CASE.FC_SHEQU_BUILD == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_SHEQU_TREE == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_SHEQU_POOL == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_SHEQU_OCEAN == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_SHEQU_SKY == fpsCaseNode.Current.Key
                    || FpsCaseManager.FPS_CASE.FC_SHEQU_TERRAIN == fpsCaseNode.Current.Key)
                {
                    m_fpsCaseManager.show(fpsCaseNode.Current.Key, !bShow);
                }
            }
        }
    }
}
