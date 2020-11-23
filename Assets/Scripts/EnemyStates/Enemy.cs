using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {



    private IEnemyState currentState;

    //enemyn target
    public GameObject Target { get; set; }
    [SerializeField]
    private float meleeRange;

    [SerializeField]
    private float throwRange;

    
    private Vector3 startPos;

    private Canvas healthCanvas;

    [SerializeField]
    private Transform leftEdge;
    [SerializeField]
    private Transform rightEdge;


    //osoittaa jos enemy on melee rangella
    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }
            return false;
        }
    }

    //osoittaa jos enemy on ampumarangella
    public bool InThrowRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }
            return false;
        }
    }

    public override bool IsDead
    {
        get
        {
            return healthStat.CurrentVal <= 0;
        }
    }



    // Use this for initialization
    public override void Start () {
        base.Start();

        //kutsutaan RemoveTarget funktioa kun pelaajan deadevent triggeröityy
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);

        //vaihtaa pelaajan idle tilaan
        ChangeState(new IdleState());
        
        healthCanvas = transform.GetComponentInChildren<Canvas>();
        
    }
	
	// Update is called once per frame
	void Update () {

        if (!IsDead)
        {

            if (!TakingDamage)
            {
                currentState.Execute();
            }

            LookAtTarget();
        }
        startPos = transform.position;
    }

    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }

    private void LookAtTarget()
    {
        if(Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;

            if(xDir < 0 && facingRight || xDir > 0 && !facingRight)
            {
                ChangeDirection();
            }
        }
    }
    
    public void ChangeState(IEnemyState newState)
    {
        if(currentState!=null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter(this);


    }

    public void Move()

        

    {
        if (!Attack)
        {

            if((GetDirection().x > 0 && transform.position.x < rightEdge.position.x)||( GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
            {
                MyAnimator.SetFloat("speed", 1);

                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            }

           
        }
        else if (currentState is PatrolState)
        {
            ChangeDirection();
        }

        }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }
   public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }

    public override IEnumerator TakeDamage()
    {
        if (!healthCanvas.isActiveAndEnabled)
        {
            healthCanvas.enabled = true;
        }
        //vähentää healthia
        healthStat.CurrentVal -= 10;

        if (!IsDead) // jos enemy ei ole kuollut, kutsutaan damage animaatioa
        {
            MyAnimator.SetTrigger("damage");
        }
        else
        {
            //jos enemy on kuollut kutsutaan dead animaatio
            MyAnimator.SetTrigger("die");
            yield return null;
        }
    }

    //poistaa pelaajan pelistä
    public override void Death()
    {
        MyAnimator.ResetTrigger("die");
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentVal = healthStat.MaxVal;
        transform.position = startPos;
        healthCanvas.enabled = false;

       
    }

    public override void ChangeDirection()
    {
        //tekee referenssin enemy canvakseen
        Transform tmp = transform.Find("EnemyHealthBarCanvas").transform;

        //tallentaa position, jotta tiedetään minne liikuttaa se kun enemy on käännetty
        Vector3 pos = tmp.position;

        ///poistaa canvaksen enemyltä, jotta health bar ei käänny enemyn mukana
        tmp.SetParent(null);

        ///vaihtaa enemyn suunnan
        base.ChangeDirection();

        //laittaa health barin takaisin enemylle
        tmp.SetParent(transform);

        //laittaa health barin takaisin oikeaan paikkaan
        tmp.position = pos;
    }
}
