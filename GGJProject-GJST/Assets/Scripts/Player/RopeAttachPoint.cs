using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttachPointType
{
    def = 0,
    headLeft = 1,
    headRight = 2,
    hearForward = 3,
    bodyLeft = 4,
    bodyRight = 5,
    station = 6,
}

/// <summary>
/// 玩家绳索狗爪抓取的地方，仅作存储位置信息功能
/// </summary>
public class RopeAttachPoint : MonoBehaviour
{

    private int myVar;

    public static List<RopeAttachPoint> AttachPointList = new List<RopeAttachPoint>();

    public float distance
    {
        get { return Vector3.Distance(PlayerControl.GameplayerIns.transform.position, transform.position); }
    }

    [SerializeField] private float forceDistance=7;

    public float ForceDistance => forceDistance;

    private Vector3 startPostion;

    [SerializeField]
    private AttachPointType pointType;

    public AttachPointType PointType { get { return pointType; } }

    [SerializeField]
    float shootDistance = float.PositiveInfinity;
    public bool isInshootDistan
    {
        get { return distance < shootDistance; }
    }

    public Rigidbody2D rb;
    //public DistanceJoint2D joint;

    // Start is called before the first frame update
    void Start()
    {
        if (RopeAttachPoint.AttachPointList.Contains(this))
        {
            return;
        }
        else
        {
            RopeAttachPoint.AttachPointList.Add(this);
        }
        rb = GetComponent<Rigidbody2D>();
        startPostion = transform.position;
        //joint = GetComponent<DistanceJoint2D>();
    }

    public void unspawnPoint()
    {
        if (RopeAttachPoint.AttachPointList.Contains(this))
        {
            RopeAttachPoint.AttachPointList.Remove(this);
        }
    }
    void Update()
    {
        switch (pointType)
        {
            case AttachPointType.def:
            case AttachPointType.headLeft:
            case AttachPointType.headRight:
            case AttachPointType.hearForward:
            case AttachPointType.bodyLeft:
            case AttachPointType.bodyRight:
                transform.transform.localPosition = startPostion;
                break;
            case AttachPointType.station:
                break;
            default:
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }


}
