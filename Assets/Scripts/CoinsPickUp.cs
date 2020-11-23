using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsPickUp : MonoBehaviour {

    public int CoinsToAdd;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        CoinsManager.AddCoins(CoinsToAdd);
        Destroy(gameObject);
    }

}

