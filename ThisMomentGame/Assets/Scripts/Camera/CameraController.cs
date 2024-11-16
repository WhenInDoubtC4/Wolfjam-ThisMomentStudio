using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;

    [Header("Camera Follow")]
    [SerializeField] bool followActive = true;
    [SerializeField] Transform cameraTarget;
    [SerializeField, Range(0, 1)] float followEasingRatio = 0.3f;
    [SerializeField] Vector3 offset = new Vector3(0, 0, -10);

    [Header("Camera Zoom")]
    [SerializeField] bool zoomActive = true;
    [SerializeField] float targetZoom;
    [SerializeField, Range(0, 1)] float zoomEasingRatio = 0.02f;

    [Header("Global Zoom")]
    [SerializeField] bool useGlobalZoom = false;
    [SerializeField] float globalZoom;

    private void OnValidate()
    {
        if(cam == null && TryGetComponent(out Camera newCam))
        {
            cam = newCam;
        }

    }

    private void FixedUpdate()
    {
        FollowTarget();
        ZoomCamera();
        MatchGlobalZoom();
    }

    void FollowTarget()
    {
        if(!followActive || cameraTarget == null)
            return;

        cam.transform.position = Vector3.Lerp(cam.transform.position, cameraTarget.position + offset, followEasingRatio);
    }

    void ZoomCamera()
    {
        if (!zoomActive)
            return;

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomEasingRatio);
    }

    void MatchGlobalZoom()
    {
        if (!useGlobalZoom)
            return;

        targetZoom = globalZoom;
    }

    public void AddToGlobalZoom(float val)
    {
        globalZoom += val;
    }




    public void SetFollowActive(bool active) { followActive = active; }
    public bool GetFollowActive() { return followActive; }

    public void SetZoomActive(bool active) { zoomActive = active; }
    public bool GetZoomActive() { return zoomActive; }

    public void SetUseGlobalZoom(bool active) { useGlobalZoom = active; }
    public bool GetUseGlobalZoom() { return useGlobalZoom; }

    public void SetGlobalZoom(float val) { globalZoom = val; }
    public float GetGlobalZoom() { return globalZoom; }

}
