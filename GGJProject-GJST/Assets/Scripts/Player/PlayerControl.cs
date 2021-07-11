using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;


public enum MoveState
{
    release,
    connected,
}

public class PlayerControl : MonoBehaviour
{
    private Rope playerRope;


    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float baseSpeed;


    public float BaseSpeed
    {
        get => baseSpeed;
        private set => baseSpeed = Train.TrainSpeed - 1f;
    }

    private float set;
    [SerializeField] Rope m_ropePrefab;
    [SerializeField] Hook m_hookPrefab;
    [SerializeField] private CinemachineTargetGroup m_CinemachineTargetGroup;

    private float inputX;
    private float inputY;
    private bool buttonA;
    private bool ButtonB;

    [HideInInspector] public List<RopeAttachPoint> ropeAttachPointLis;
    public static PlayerControl GameplayerIns;

    DistanceJoint2D DisJoint;
    GameObject ropeObj;

    private RopeAttachPoint nearlistPoint;


    public RopeAttachPoint NearlistPoint
    {
        get { return nearlistPoint; }
    }


    #region getTrainRefPos

    [SerializeField] TrainController Train;


    public Vector3 RefTrainPostion
    {
        get
        {
            if (Train != null)
            {
                Vector3 destinationFromTrainToPlayer = transform.position - Train.transform.position;
                Vector3 PojectVecOnTrainDestination =
                    Vector3.Project(destinationFromTrainToPlayer, Train.transform.right);

                return PojectVecOnTrainDestination;
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }
    }

    public Vector3 showRef;
    public Vector3 showREfVEc;

    #endregion

    public SkeletonAnimation[] thrusterList;

    [SpineAnimation] public string thrusterStopAnim;
    [SpineAnimation] public string thrusterWorkAnim;

    // public void GetTrainVec()
    // {
    //     refTrainPostion = transform.position - Train.transform.up;
    // }
    void Start()
    {
        PlayerControl.GameplayerIns = this;
        InitRope();
        InitHook();
    }
    void SetAnim()
    {

        if (inputX>0&&inputY==0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterWorkAnim, true);
        }
        if (inputX < 0 && inputY == 0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterStopAnim, true);
        }
        if (inputX == 0 && inputY > 0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterWorkAnim, true);
        }
        if (inputX== 0 && inputY < 0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterStopAnim, true);
        }

        if (inputX>0&&inputY>0)
        {

            thrusterList[0].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterWorkAnim, true);

        }
        if(inputX<0&&inputY>0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterStopAnim, true);
        }

        if (inputX<0&&inputY<0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterStopAnim, true);
        }
        if (inputX > 0 && inputY < 0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterWorkAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterStopAnim, true);
        }
        if (inputY==0&&inputX==0)
        {
            thrusterList[0].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[1].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[2].state.SetAnimation(0, thrusterStopAnim, true);
            thrusterList[3].state.SetAnimation(0, thrusterStopAnim, true);
        }


    }

    public void Init(int speed)
    {
        playerMoveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        showRef = RefTrainPostion;
        GetInput();
        Move();
        TrainControl();
        showREfVEc = RefTrainPostion;
        ChangeCamera();
        SetAnim();
    }

    void ChangeCamera()
    {
        if (math.distance(this.transform.position, Train.transform.position) <= 10)
        {
            if (m_CinemachineTargetGroup.FindMember(Train.transform) < 0)
            {
                m_CinemachineTargetGroup.AddMember(Train.gameObject.transform, 0, 5);
            }
        }
        else
        {
            if (m_CinemachineTargetGroup.FindMember(Train.transform) >= 0)
            {
                m_CinemachineTargetGroup.RemoveMember(Train.gameObject.transform);
            }
        }
    }

    #region Controller

    public delegate void ButtonAHandler(int args);

    public event ButtonAHandler ButtonAevent;

    private void GetInput()
    {
        // Vector3.SignedAngle()
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.J))
            OnButtonAPush();
    }

    public void OnButtonAPush()
    {
        // Debug.Log("buttonAPushed");
        if (Rope.ropeIns)
        {
            if (Rope.ropeIns.setOn)
            {
                Rope.ropeIns.setOn = false;
                Hook.hookIns.ShutDownHook();
            }
            else
            {
                BubblePointLis();
                if (NearlistPoint.isInshootDistan) //在可以抓起的范围内才能发射绳索
                {
                    Rope.ropeIns.Init(nearlistPoint.transform);
                    Hook.hookIns.ActiveHook(nearlistPoint);
                    Rope.ropeIns.setOn = true;
                }
            }
        }
    }

    [ContextMenu("test")]
    public RopeAttachPoint BubblePointLis()
    {
        nearlistPoint = null;
        ropeAttachPointLis = RopeAttachPoint.AttachPointList;
        if (ropeAttachPointLis.Count == 0)
        {
            Debug.Log("No attachPoint detected");
            return null;
        }

        int length = ropeAttachPointLis.Count - 1;
        RopeAttachPoint min = ropeAttachPointLis[0];
        for (int i = 0; i < length; i++)
        {
            if (min.distance > ropeAttachPointLis[i + 1].distance)
            {
                min = ropeAttachPointLis[i + 1];
            }
        }

        nearlistPoint = min;
        return min;
    }

    public void InitHook()
    {
        GameObject HookPrefab = m_hookPrefab.gameObject;
        HookPrefab.SetActive(true);
        //Instantiate(HookPrefab);
    }

    public void InitRope()
    {
        nearlistPoint = BubblePointLis();
        if (nearlistPoint == null)
        {
            Debug.Log("NearlistPoint is null");
            return;
        }

        GameObject playerRope = m_ropePrefab.gameObject;
        Rope rope = playerRope.GetComponent<Rope>();
        rope.setOn = false;
        //if (nearlistPoint.isInshootDistan)
        //{
        //    nearlistPoint.joint.enabled = true;//激活挂点
        //    rope.setOn = true;
        //    rope.Init(nearlistPoint.transform);
        //}
        Instantiate(playerRope);
    }


    private void Move()
    {
        if (inputX != 0)
        {
            transform.position += new Vector3(inputX * playerMoveSpeed * Time.deltaTime, 0, 0);
        }


        transform.position += new Vector3(0, (Train.TrainSpeed-1 + inputY * playerMoveSpeed) * Time.deltaTime, 0);
    }

    private void TrainControl()
    {
        if (!Rope.ropeIns.setOn)
            return;
        Vector3 refVec = transform.position - NearlistPoint.transform.position;


        switch (nearlistPoint.PointType)
        {
            case AttachPointType.def:
            case AttachPointType.headLeft:
            case AttachPointType.headRight:
            case AttachPointType.bodyLeft:
            case AttachPointType.bodyRight:
                PlayerTrainControl();
                break;
            default:
                break;
        }
    }


    [SerializeField] private float pullPower;

    private void PlayerTrainControl()
    {
        if (nearlistPoint.distance > nearlistPoint.ForceDistance)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                for (int i = 0; i < pullPower; i++)
                {
                    Train.ApplyPower(Mathf.Sign(-RefTrainPostion.x));
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 3);
    }

    #endregion
}