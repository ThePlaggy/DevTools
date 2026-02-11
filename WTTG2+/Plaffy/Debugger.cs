using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plaffy
{
    public class Debugger : MonoBehaviour
    {
        private Vector2 DebuggerPosition = new Vector2(0, 0);
        private Image PFDI;
        private TextMeshProUGUI PFDT;
        private float geschwindigkeit = 400f;
        private void Start()
        {
            Canvas PFDUI = new GameObject("PlaffyDebuggerUI").AddComponent<Canvas>();
            PFDT = 
            new GameObject("PlaffyDebuggerText").AddComponent<TextMeshProUGUI>();
            PFDI =
            new GameObject("PlaffyDebuggerImage").AddComponent<Image>();
            PFDUI.renderMode = RenderMode.ScreenSpaceOverlay;
            PFDUI.gameObject.AddComponent<CanvasScaler>();
            PFDUI.gameObject.AddComponent<GraphicRaycaster>();
            PFDUI.sortingOrder = 5;
            PFDT.transform.SetParent(PFDUI.transform);
            PFDI.transform.SetParent(PFDI.transform);
            PFDI.rectTransform.localPosition = DebuggerPosition;
            PFDI.rectTransform.sizeDelta = new Vector2(1920, 1080);
            PFDT.transform.localPosition = DebuggerPosition;
            PFDT.rectTransform.sizeDelta = new Vector2(1920, 1080);
            PFDT.rectTransform.localPosition = Vector3.zero;
            PFDT.font = 
            Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            PFDT.fontSize = 18;
            PFDT.color = Color.white;
            Application.logMessageReceived += LogRegister;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                DebuggerPosition.y -= geschwindigkeit * Time.deltaTime;
                PFDI.rectTransform.localPosition = DebuggerPosition;
                PFDT.transform.localPosition = DebuggerPosition;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                DebuggerPosition.y += geschwindigkeit * Time.deltaTime;
                PFDI.rectTransform.localPosition = DebuggerPosition;
                PFDT.transform.localPosition = DebuggerPosition;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                DebuggerPosition.x -= geschwindigkeit * Time.deltaTime;
                PFDI.rectTransform.localPosition = DebuggerPosition;
                PFDT.transform.localPosition = DebuggerPosition;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                DebuggerPosition.x += geschwindigkeit * Time.deltaTime;
                PFDI.rectTransform.localPosition = DebuggerPosition;
                PFDT.transform.localPosition = DebuggerPosition;
            }
            if(Input.GetKeyDown(KeyCode.Alpha0))
            {
                PFDI.enabled = !PFDI.enabled;
                PFDT.enabled = !PFDT.enabled;
            }
        }

        private void LogRegister(string logString, string stackTrace, LogType type)
        {
            PFDT.text += logString + "\n";
            string[] lines = PFDT.text.Split('\n');
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= LogRegister;
        }
    }
}