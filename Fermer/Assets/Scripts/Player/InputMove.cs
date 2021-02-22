using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputMove : MonoBehaviour
{
    [SerializeField, Range(1,10)] private float speed = 5f;
    [SerializeField, Range(1, 50)] private float jumpForce = 15.0f;
    [SerializeField, Range(-40, -1)] private float terminalVelocity = -10.0f;
    [SerializeField, Tooltip("Сила притяжения на земле"), Range(-2, 0)] private float minFall = -1.5f;
    [SerializeField, Range(0.1f, 20)] private float gravity = 9.8f;
    [SerializeField, Range(1, 5), Tooltip("Ускорение при рывке")] private float sprintMultiplicator = 3;
    [SerializeField, Range(0.1f, 1), Tooltip("Время рывка")] private float sprintTime = 0.5f;
    [SerializeField, Range(1,5), Tooltip("Время перезарядки рывка")] private float sprintReloadTime = 2;

    private CharacterController charController;
    private Vector3 moveVector;
    private Vector3 horSpeed;
    private float sprintMultiplicatorBufer;
    private float currentSprintReloadTime;
    private float vertSpeed;
    private int sprintCount;
    private bool inMenu;
    private bool startup;
    private bool fall;
    private float fallTimer;

    void Awake()
    {
        PlayerBonusStat.Init();
        Messenger<bool>.AddListener(GameEvent.PAUSE, OnPause);
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_JUMP, OnTakeBonusJump);
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_SPEED, OnTakeBonusSpeed);
    //    Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }
    void OnDestroy()
    {
        Messenger<bool>.RemoveListener(GameEvent.PAUSE, OnPause);
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_JUMP, OnTakeBonusJump);
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_SPEED, OnTakeBonusSpeed);
     //   Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }

    private void Start()
    {
        vertSpeed = minFall;
        charController = GetComponent<CharacterController>();
        sprintMultiplicatorBufer = 1;
        fall = true;
    }
    void Update()
    {
        if(!inMenu)
        {
            Jump();
            PlayerSprint();
            PlayerMove();
        }
    }

    public void Setup(Transform targetPos)
    {
        startup = true;
        moveVector = Vector3.zero;
        transform.position = targetPos.position;
        transform.rotation = targetPos.rotation;
        Invoke("StopStartup", 0.5f);
    }

    private void OnTakeBonusJump(int value)
    {
        PlayerBonusStat.bonusPack[BonusType.Jump] = value;
    }
    private void OnTakeBonusSpeed(int value)
    {
        PlayerBonusStat.bonusPack[BonusType.Speed] = value;
    }

    private void Jump()
    {
        if (charController.isGrounded)
        {
            fallTimer = 0;
            fall = true;
            if (Input.GetButtonDown("Jump"))
            {
                vertSpeed = jumpForce;
            }
            else
            { 
                vertSpeed = minFall;
            }
        }
        else
        {
            if (fall)
            {
                vertSpeed -= gravity * 5 / PlayerBonusStat.bonusPack[BonusType.Jump] * Time.deltaTime;
                if (vertSpeed < terminalVelocity)
                {
                    vertSpeed = terminalVelocity;
                }
            }
            else
            {
                fallTimer -= Time.deltaTime;
                if(fallTimer <= 0)
                {
                    fallTimer = 0;
                    fall = true;
                }
                vertSpeed = 0;
            }
        }
    }
    private void PlayerMove()
    {
        if(!startup)
        {
            float deltaX = Input.GetAxis("Horizontal") * speed;
            float deltaZ = Input.GetAxis("Vertical") * speed;
            moveVector = new Vector3(deltaX, 0, deltaZ);
            moveVector = Vector3.ClampMagnitude(moveVector, speed) * sprintMultiplicatorBufer * PlayerBonusStat.bonusPack[BonusType.Speed]; //Ограничим движение по диагонали той же скоростью, что и движение параллельно осям
            horSpeed = moveVector;
            moveVector.y = vertSpeed;
            moveVector *= (Time.deltaTime);
            moveVector = transform.TransformDirection(moveVector); //Преобразуем вектор движения от локальных к глобальным координатам.

            charController.Move(moveVector);
        }
    }
    private void PlayerSprint()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && horSpeed.magnitude != 0 && sprintMultiplicatorBufer == 1)
        {
            if(sprintCount > 0)
            {
                fall = false;
                fallTimer += sprintTime + 0.1f;
                sprintMultiplicatorBufer = sprintMultiplicator;
                Invoke("ReturnSprintOpportunity", sprintTime);
                Invoke("StopSprint", sprintTime + 0.1f);
                sprintCount--;
                currentSprintReloadTime = sprintReloadTime;
                Messenger<int>.Broadcast(GameEvent.CHANGE_SPRINT_COUNT, sprintCount);
                Messenger.Broadcast(GameEvent.SPRINT_ACTION);
                Messenger<Vector3>.Broadcast(GameEvent.START_SPRINT, horSpeed);
            }
        }
        if(sprintCount < 3)
        {
            currentSprintReloadTime -= Time.deltaTime;
            if(currentSprintReloadTime <= 0)
            {
                sprintCount++;
                currentSprintReloadTime = sprintCount == 3 ? 0 : sprintReloadTime;
                sprintMultiplicatorBufer = 1;
                Messenger<int>.Broadcast(GameEvent.CHANGE_SPRINT_COUNT, sprintCount);
            }
        }
    }

    private void OnPause(bool pause)
    {
        inMenu = pause;
    }

    #region Вызовы Invoke
    private void StopStartup() => startup = false;
    private void ReturnSprintOpportunity()=> sprintMultiplicatorBufer = 1;
    private void StopSprint()
    {
        Messenger.Broadcast(GameEvent.STOP_SPRINT);
    }
    #endregion
}
