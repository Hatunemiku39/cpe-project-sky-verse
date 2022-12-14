using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum handToTrack
{
    Left,
    Right
}

public class handdrawing1 : MonoBehaviour
{
    public handToTrack handtoTrack = handToTrack.Right;

    public GameObject objectToTrackMovement;

    private Vector3 prevPointDistance = Vector3.zero;

    [Range(0, 1.0f)]
    public float lineDefaultWidth = 0.010f;

    public float minFingerPinchStrenth = 0.5f;

    [Range(0, 1.0f)]
    public float minDistanceBeforeNewPoint = 0.2f;

    private int positionCount = 0;

    private List<LineRenderer> Lines = new List<LineRenderer>();

    private LineRenderer currentLineRenderer;

    public Color defaultColor = Color.red;

    public Material defaultMaterial;

    private bool isPinchDown = false;

    //For hand tracking

    private OVRHand ovrHand;

    private OVRSkeleton OVRSkeleton;

    private OVRBone boneToTrack;
    private Material defaultLineMaterial;

    private void Awake()
    {
        ovrHand = objectToTrackMovement.GetComponent<OVRHand>();
        OVRSkeleton = objectToTrackMovement.GetComponent<OVRSkeleton>();
       
    }

    void AddNewLineRenderer()
    {
        positionCount = 0;

        GameObject go = new GameObject("Line");
        go.transform.parent = objectToTrackMovement.transform.parent;
        go.transform.position = objectToTrackMovement.transform.position;

        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineDefaultWidth;
        goLineRenderer.endWidth = lineDefaultWidth;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.material = defaultLineMaterial;
        goLineRenderer.positionCount = 1;
        goLineRenderer.numCapVertices = 5;

        currentLineRenderer = goLineRenderer;
        Lines.Add(goLineRenderer);

    }

   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPinchState();
    }

    void CheckPinchState()
    {
        bool isIndexFingerPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        float indexFingerPinchStrength = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        if(isIndexFingerPinching && indexFingerPinchStrength >= minFingerPinchStrenth)
        {
            UpdateLine();
            isPinchDown = true;
            return;
        }

        if(isPinchDown)
        {
            AddNewLineRenderer();
            isPinchDown = false;
        }
    }

    void UpdateLine()
    {
        if(prevPointDistance == null)
        {
            prevPointDistance = objectToTrackMovement.transform.position;
        }

        if(prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, objectToTrackMovement.transform.position))>= minDistanceBeforeNewPoint)
        {
            Vector3 dir = (objectToTrackMovement.transform.position - Camera.main.transform.position).normalized;
            prevPointDistance = objectToTrackMovement.transform.position;
            AddPoint(prevPointDistance, dir);
        }
    }

    void AddPoint(Vector3 position, Vector3 direction)
    {
        currentLineRenderer.SetPosition(positionCount, position);
        positionCount++;
        currentLineRenderer.positionCount = positionCount + 1;
        currentLineRenderer.SetPosition(positionCount, position);
    }
}