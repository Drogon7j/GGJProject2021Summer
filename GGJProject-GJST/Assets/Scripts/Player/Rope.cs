using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    public float MaxLength;

    private float currentLength;
    [SerializeField]
    Transform PosTrain;
    [SerializeField]
    Transform PosPlayer;

    [SerializeField]
    float lengthVar=1;

    public bool setOn = false;

    public static Rope ropeIns;

    public bool canDrawLine
    {
        get
        {
            return PosPlayer != null && PosPlayer != null&&setOn;
        }
    }

    public float distance
    {
        get
        {

            if (canDrawLine)
            {
                return Vector3.Distance(PosPlayer.transform.position,PosTrain.transform.position);
            }
            else
            {
                return 0;
            }
        }
    }


     public void Init(Transform Train)
    {
        PosTrain = Train;
    }
    // Start is called before the first frame update
    void Start()
    {
        ropeIns = this;
        PlayerControl player = PlayerControl.GameplayerIns;
        PosPlayer = player.transform;
        if (player.NearlistPoint != null)
        {
           PosTrain=player.NearlistPoint.transform ;
        }
        currentLength = Vector3.Distance(PosTrain.position, PosPlayer.position);

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1, distance* lengthVar, 1);

        if (canDrawLine)
        {
            transform.position = (PosPlayer.position + PosTrain.position) / 2;

            Vector3 v = PosPlayer.position - transform.position;
            v.z = 0;
            float angle = Vector3.SignedAngle(Vector3.up, v, Vector3.forward);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = rotation;

        }
    }

    public void destroySelf()
    {
        GameObject.DestroyImmediate(this);
    }
}
