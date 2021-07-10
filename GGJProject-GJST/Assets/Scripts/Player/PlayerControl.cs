using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum MoveState
{
    
}
public class PlayerControl : MonoBehaviour
{
    private Rope playerRope;

    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float set;

    private float inputX;
    private float inputY;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(int speed)
    {
        playerMoveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
    }

    #region Controller

    private void GetInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        if (inputX!=0)
        {
            transform.position += new Vector3(inputX * playerMoveSpeed*Time.deltaTime,0,0);
            
        }

        if (inputY!=0)
        {
            transform.position += new Vector3(0, inputY*playerMoveSpeed*Time.deltaTime, 0);
        }
    }
    

    #endregion
}
