using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMobSpell : MonoBehaviour
{
    public GameObject sprite;
    public GameObject range;
    private CapsuleCollider2D collider;

    void Start()
    {
        collider = GetComponent<CapsuleCollider2D>();
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1.5f);

        range.SetActive(false);
        sprite.SetActive(true);
        yield return new WaitForSeconds(0.7f);

        collider.enabled = true;
        yield return new WaitForSeconds(0.6f);

        collider.enabled = false;
        yield return new WaitForSeconds(0.6f);

        sprite.SetActive(false);
    }
}
