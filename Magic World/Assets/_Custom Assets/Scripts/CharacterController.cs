﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpellSystem;
using UnityEngine.AI;
using ComboSystem;
using CombatSystem;
using SkillSystem;
using InputSystem;

public class CharacterController : MonoBehaviour {

    CharacterMovement controller;
    CombatController combatController;
    SkillCaster spellCaster;
    new Camera camera;
    Animator animator;
    NavMeshAgent agent;
    CharacterCombos combos;
    CharacterManager manager;

    float movementSpeed = 5f;
    bool movementLocked = false;
    Camera mainCamera;

    private Vector3 lastPosition;

    // Use this for initialization
    void Start () {
        controller = transform.GetComponentInChildren<CharacterMovement>();
        combatController = transform.GetComponent<CombatController>();
        spellCaster = transform.GetComponent<SkillCaster>();
        animator = transform.GetComponentInChildren<Animator>();
        agent = transform.GetComponent<NavMeshAgent>();
        combos = transform.GetComponent<CharacterCombos>();
        manager = transform.GetComponent<CharacterManager>();
        camera = Camera.main;

        lastPosition = CopyVector(transform.position);
	}
	
	// Update is called once per frame
	void Update () {

        movementLocked = combatController.lockedMovement;

        //sets variables for movement animation
        float speed = CharacterSpeed();
        animator.SetFloat("Speed", speed);
        animator.SetBool("Moving", IsCharacterMoving());

        //guard when not moving
        if (!movementLocked && agent.velocity.magnitude == 0 && !combatController.guarding)
        {
            if (InputManager.manager.GetKeyDown("Dodge"))
            {
                combatController.Guard();
            }
        }

        if (combatController.guarding) {
            if (InputManager.manager.GetKeyUp("Dodge")) {
                combatController.Unguard();
            }
        }

        //movement
        if (!movementLocked)
        {
            Vector3 direction = DirectionFromInput();
            //checks whether player is guarding or not when moving
            
            if (!combatController.guarding)
            {
                controller.Move(direction, movementSpeed);
            }
            else
            {
                controller.Move(direction, combatController.guardMoveSpeed);
            }
        }

        //Cast spells
        if (InputManager.manager.GetKeyDown("Cast")) {
            manager.caster.CastSpell();
        }


        //Attack with left mouse or Square
        if (InputManager.manager.GetKeyDown("Attack"))
        {
            if (!movementLocked)
            {
                combos.Attack();
            }
            else {
                if (combatController.bufferOpen) {
                    combatController.bufferedAction = Action.Attack;
                }
            }
        }
        

        /*
        if (Input.GetKeyDown(KeyCode.H)) {
            animator.Play("Flinch", 2);
        }*/

    }


    public void FixedUpdate()
    {
        //dodge with Q
        if (InputManager.manager.GetKeyDown("Dodge") && agent.velocity.magnitude != 0)
        {
            if (combatController.currentState == Action.Guard)
            {
                combatController.Unguard();
            }

            if (!movementLocked)
            {
                StartCoroutine(combatController.Dodge(DirectionFromInput()));
            }
            else {
                if (combatController.bufferOpen)
                {
                    combatController.bufferedAction = Action.Dodge;
                }
            }
        }


        //calls buffered action
        if (combatController.bufferedAction != Action.None && !combatController.lockedMovement)
        {
            switch (combatController.bufferedAction)
            {
                case Action.Attack:
                    //controller.Rotate(DirectionFromInput());
                    combos.Attack();
                    break;
                case Action.Dodge:
                    controller.Rotate(DirectionFromInput());
                    StartCoroutine(combatController.Dodge(DirectionFromInput()));
                    break;
                case Action.Skill:
                    break;
            }
            combatController.bufferedAction = Action.None;
        }
    }



    /// <summary>
    /// Gets a direction from the input
    /// </summary>
    /// <returns></returns>
    public Vector3 DirectionFromInput()
    {
        Vector3 cameraForward = camera.transform.forward;
        cameraForward = new Vector3(cameraForward.x, 0, cameraForward.z);
        cameraForward /= cameraForward.magnitude;
        Vector3 cameraRight = camera.transform.right;
        cameraRight = new Vector3(cameraRight.x, 0, cameraRight.z);
        cameraRight /= cameraRight.magnitude;
        Vector3 direction = cameraForward * InputManager.manager.GetAxis("Vertical Left");
        direction += cameraRight * InputManager.manager.GetAxis("Horizontal Left");

        if (direction.magnitude > 1)
        {
            direction /= direction.magnitude;
        }

        return direction;

    }



    public void LockMovement() {
        movementLocked = true;
    }



    /*
    /// <summary>
    /// Locks movement for lockTime. Returns false if movement is already locked.
    /// </summary>
    /// <param name="lockTime"></param>
    /// <returns></returns>
    public bool TimedLockedMovement(float lockTime) {

        bool lockWorked = true;

        if (!movementLocked)
        {
            lockTimer = lockTime;
            movementLocked = true;
        }
        else {
            lockWorked = false;
        }

        return lockWorked;
    }
    
    */
    public void UnlockMovement() {
        movementLocked = false;
    }


    //determines whether a character is moving for animation purposes
    private bool IsCharacterMoving() {
        return DirectionFromInput().magnitude != 0;
    }

    //calculate a character's speed
    private float CharacterSpeed() {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = CopyVector(transform.position);
        return speed;
    }

    private Vector3 CopyVector(Vector3 newVector) {
        return new Vector3(newVector.x, newVector.y, newVector.z);
    }

}
