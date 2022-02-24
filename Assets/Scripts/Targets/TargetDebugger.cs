#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TargetDebugger : MonoBehaviour
{
    private Ray _ray;
    private RaycastHit _hit;

    private void OnEnable()
    {
        if (!Application.isEditor)
            Destroy(this);
        SceneView.duringSceneGui += OnScene;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnScene;
    }

    void OnScene(SceneView scene)
    {
        if (!Application.isPlaying)
            return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;

            _ray = scene.camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.transform != null)
                {
                    TargetManager tm = _hit.transform.GetComponent<TargetManager>();
                    if (tm != null)
                        _hit.transform.GetComponent<TargetManager>().TakeHit();
                }
            }
            e.Use();
        }
    }

}
#endif
