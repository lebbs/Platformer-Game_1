using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Character : MonoBehaviour {

    [SerializeField]
    protected Transform TuliPalloPos;

    [SerializeField]
    protected float movementSpeed;

    protected bool facingRight;

    [SerializeField]
    private GameObject tuliPalloPrefab;

    //characterin health
    [SerializeField]
    protected Stat healthStat;

    //characterin swordcollider
    [SerializeField]
    private EdgeCollider2D swordCollider;

    [SerializeField]
    private List<string> damageSources;

    public abstract bool IsDead { get; }

    public bool Attack { get; set; }

    public bool TakingDamage { get; set; }

    public Animator MyAnimator { get; private set; }

    public EdgeCollider2D SwordCollider
    {
        get
        {
            return swordCollider;
        }
        
    }



    // Use this for initialization
    public virtual void Start () {
        facingRight = true;
        MyAnimator = GetComponent<Animator>();
        healthStat.Initialize();

    }

    // Update is called once per frame
    void Update () {
		
	}

    public abstract IEnumerator TakeDamage();

    public abstract void Death();

        //vaihtaa characterin suunnan
    public virtual void ChangeDirection()
    {
        //vaihtaa facingright arvon negatiiviseksi
        facingRight = !facingRight;

        //käännetään character muuttamalla scalea
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

        //tulipallon ampuminen
    public virtual void ThrowBall(int value)
    {
        if (facingRight) //jos katse vasempaan --> tulipallo ammutaan vasemmalle
        {
            GameObject tmp = (GameObject)Instantiate(tuliPalloPrefab, TuliPalloPos.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            tmp.GetComponent<TuliPallo>().Initialize(Vector2.right);
        }
        else // muulloin oikealle
        {
            GameObject tmp = (GameObject)Instantiate(tuliPalloPrefab, TuliPalloPos.position, Quaternion.Euler(new Vector3(0, 0, -180)));
            tmp.GetComponent<TuliPallo>().Initialize(Vector2.left);
        }
    }

    public void MeleeAttack()
    {
        SwordCollider.enabled = true;
    }
    

   public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }
        if (other.gameObject.tag == "Spike")
        {
            Death();
        }
        if (other.gameObject.tag == "Mace")
        {
            Death();
        }
        if (other.gameObject.tag == "Saw")
        {
            Death();
        }


    }
}
