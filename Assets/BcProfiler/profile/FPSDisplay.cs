using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EngineProfiler
{
    //ref: http://wiki.unity3d.com/wiki/index.php?title=FramesPerSecond
    public class FPSDisplay : MonoBehaviour
    {

        private float lastTime_;
        private int frameCount_;
        private static float interval_ = 1.0f;
        public static float fps_;
        public int targetFrameRate = 60;
        public Text fpsText = null;
        public Text neckText = null;

        GUIStyle style;
        Rect rect;
        float cpuneckTimeCount_ = 0;
        float cpuneckTime_ = 0;

        void Start()
        {
            int w = Screen.width, h = Screen.height;
            rect = new Rect(w * 0.5f, 20, w, h * 2 / 50);

            style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.normal.textColor = Color.yellow;
        }
        void Update()
        {
            Application.targetFrameRate = targetFrameRate;
            cpuneckTimeCount_ += CpuNeck.time_;
            frameCount_++;
            if (Time.unscaledTime - lastTime_ > interval_)
            {
                fps_ = (float)frameCount_ / (Time.unscaledTime - lastTime_);
                lastTime_ = Time.unscaledTime;
                cpuneckTime_ = cpuneckTimeCount_ / frameCount_;
                cpuneckTimeCount_ = 0;
                frameCount_ = 0;
            }

            if (null != fpsText)
                fpsText.text = string.Format("{0:f1}", fps_);
            if (null != neckText)
                neckText.text = string.Format("{0:f1}ms", FpsProfilerSetting.bGpuNeck ? FpsProfiler.gpuNeckTime : cpuneckTime_);
        }

        //void OnGUI()
        //{
        //    Application.targetFrameRate = targetFrameRate;
        //    string text = string.Format("{0:f2}", fps_);
        //    GUI.Label(rect, text, style);
        //}
    }
}