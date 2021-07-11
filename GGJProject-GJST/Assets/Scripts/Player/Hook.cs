using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum HookState
{
    shutDown=0,
    active=1,
}

public class Hook : MonoBehaviour
{
    [SerializeField]
    float hookStableTime=1f;
    public float WarnintTime;
    float AutoDisConnetcTime;
    
    private RopeAttachPoint m_anchorPoint;
    public RopeAttachPoint AnchorPoint
    {
        get { return m_anchorPoint; }
    }
    public bool Ishooked
    {
        get { return m_anchorPoint != null; }
    }

    public static Hook hookIns;

    private HookState hookState = HookState.shutDown;
 

    Transform pointOrinParentTransform;

    DistanceJoint2D DistanceJoint;

    private void Awake()
    {
        DistanceJoint = GetComponent<DistanceJoint2D>();
        hookIns = this;
    }

    public void ActiveHook(RopeAttachPoint point)
    {
        hookState = HookState.active;
        DistanceJoint.enabled = true;
        m_anchorPoint = point;
        //DistanceJoint.distance = point.distance;

        switch (m_anchorPoint.PointType)
        {
            #region case train:
            case AttachPointType.def:
            case AttachPointType.headLeft:
            case AttachPointType.headRight:
            case AttachPointType.hearForward:
            case AttachPointType.bodyLeft:
            case AttachPointType.bodyRight:
                #endregion
                    break;
            case AttachPointType.station:


                transform.position = m_anchorPoint.transform.position;
                pointOrinParentTransform = point.transform.parent;
                m_anchorPoint.transform.parent = transform;
                m_anchorPoint.transform.localPosition = new Vector3(0, 0, 0);
                break;
            default:
                break;
        }

        ActiveScopeSet();

    }
    public void ShutDownHook()
    {
        hookState = HookState.shutDown;
        DistanceJoint.enabled = false;

        switch (m_anchorPoint.PointType)
        {
            #region case train:
            case AttachPointType.def:
            case AttachPointType.headLeft:
            case AttachPointType.headRight:
            case AttachPointType.hearForward:
            case AttachPointType.bodyLeft:
            case AttachPointType.bodyRight:
                #endregion
                break;
            case AttachPointType.station:
                m_anchorPoint.transform.parent = null;
                m_anchorPoint.transform.position = transform.position;
                pointOrinParentTransform = null;
                break;
            default:
                break;
        }
        m_anchorPoint = null;

    }
    
    void Update()
    {
        if (m_anchorPoint!=null)
        {

            switch (m_anchorPoint.PointType)
            {
                #region case train
                case AttachPointType.def:
                case AttachPointType.headLeft:
                case AttachPointType.headRight:
                case AttachPointType.hearForward:
                case AttachPointType.bodyLeft:
                case AttachPointType.bodyRight:
                    #endregion
            transform.position = m_anchorPoint.transform.position;
                    break;
                case AttachPointType.station:
                    m_anchorPoint.transform.localPosition = new Vector3(0, 0, 0);
                    break;
                default:
                    break;
            }
        }
        Yichangjiance();
        RopeUpdateSet();
    }

    
    void ActiveScopeSet()
    {
        AutoDisConnetcTime = Time.time + hookStableTime;
        WarnintTime = AutoDisConnetcTime - 0.5f;
        Rope.ropeIns.GetComponentInChildren<SpriteRenderer>().color = new Color(255 / 255f, 255 / 255f,1);


    }
    void RopeUpdateSet()
    {
        if (Time.time>WarnintTime)
        {
            Rope.ropeIns.GetComponentInChildren<SpriteRenderer>().color = new Color(255 / 255f, 29 / 255f, 0f);
            //变色
        }

        if ((Time.time>AutoDisConnetcTime)&&Rope.ropeIns.setOn == true)
        {
            Rope.ropeIns.setOn = false;
            ShutDownHook();
        }
    }
    void Yichangjiance()
    {
        if (m_anchorPoint==null&&hookState==HookState.active)
        {
            ShutDownHook();
        }
    }

}
