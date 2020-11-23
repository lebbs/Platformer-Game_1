using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove1 : MonoBehaviour {
    
    public float moveSpeed;
    // private PlayerHealth player;

    // Use this for initialization
    void Start () {
         // player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
	
	// Update is called once per frame
	void Update () {

        transform.Translate(new Vector3(moveSpeed, 0, 0) * Time.deltaTime);
		
	}

    void OnCollisionEnter2D (Collision2D col)
    {

        if (col.gameObject.tag == "Block")
        {
            moveSpeed *= -1;
            Vector3 theScale = transform.localScale;

            theScale.x *= -1;

            transform.localScale = theScale;
        }
        if (col.gameObject.tag == "Player")
        {
            /* Destroy (col.gameObject); */
            col.gameObject.transform.position = new Vector3(0, 0, 0);
        }
        
        
    }
}

/* internal class PlayerHealth
{
} */