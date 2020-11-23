﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//kerrotaan vihollisille että pelaaja on kuollut
public delegate void DeadEventHandler();

public class Player : Character
{

    private static Player instance;

    public event DeadEventHandler Dead;


    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }
    

    [SerializeField]
    private Transform[] groundPoints;

    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    

    [SerializeField]
    private bool airControl;

    [SerializeField]
    private float jumpForce;

    //kuolemattomuus
    private bool immortal = false;
    [SerializeField]
    private float immortalTime;

    private SpriteRenderer spriteRenderer;

    public Rigidbody2D MyRigidbody { get; set; }




    public bool Slide { get; set; }

    public bool Jump { get; set; }

    public bool OnGround { get; set; }

    public override bool IsDead
    {
        get
        {
            if (healthStat.CurrentVal <= 0)
            {
                OnDead();
            }
            
            return healthStat.CurrentVal <= 0;
        }
    }


    // Use this for initialization
    public override void Start () {

        base.Start();
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        MyRigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector3 startPos;
    
    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= -14f)
            {
                Death();
            }
            HandleInput();
        }
        
    }
    
	
	// Update is called once per frame
	void FixedUpdate () {

        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");

            OnGround = IsGrounded();

            HandleMovement(horizontal);

            Flip(horizontal);

            HandleLayers();
        }
	}

    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }

    //Liikkuminen
    private void HandleMovement(float horizontal)
    {

        if (MyRigidbody.velocity.y < 0)
        {
            MyAnimator.SetBool("land", true);
        }
        if(!Attack && !Slide && (OnGround || airControl))
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
        }

        if(Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }
        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    
    //Näppäin bindit
    private void HandleInput()
    {


        if(Input.GetKeyDown(KeyCode.Space))
        {
            MyAnimator.SetTrigger("jump");
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            MyAnimator.SetTrigger("attack");
        }
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            MyAnimator.SetTrigger("slide");
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            MyAnimator.SetTrigger("throw");
            
        }
    }
    //Kääntää hahmon
    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            ChangeDirection();
        }
    }
    
  


private bool IsGrounded()
    {
        if (MyRigidbody.velocity.y<=0)
        {
            foreach(Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);


                for(int i=0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject!=gameObject)
                    {
                       
                        return true;
                    }
                }

            }
            
        }
        return false;
    }

    private void HandleLayers()
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
        }
    }
   
    public override void ThrowBall(int value)
    {

        if(!OnGround && value ==1 || OnGround && value == 0)
        {
            base.ThrowBall(value);
        }

        
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override IEnumerator TakeDamage()
    {
        if (!immortal)
        {
            healthStat.CurrentVal -= 40;

            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");
                immortal = true;

                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);

                immortal = false;
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
            }
        }
    }

    public override void Death()
    {
        MyRigidbody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentVal = healthStat.MaxVal;
        transform.position = startPos;
    }
}
