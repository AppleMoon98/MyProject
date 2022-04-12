using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMob : MonoBehaviour
{
    public Player target;
    public GameObject attackArea;
    public GameObject spellProps;
    public float mobAttackRange;
    public float mobAttackDelay;
    public float mobSpellDelay;
    public float mobFieldOfVision;
    public int mobHealth;

    private Animator anim;
    private Animator shadowAnim;
    private SpriteRenderer rand;
    private SpriteRenderer shadowRand;
    private Rigidbody2D rigid;
    private Vector3 moveTarget;

    private bool b_Hurt;
    private bool b_IsAttack;
    private bool b_IsCast;

    private float f_Distance;
    private float f_AttackDistance;
    private float f_AattackDelay;
    private float f_SpellDelay;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        rand = GetComponent<SpriteRenderer>();

        // 자식 오브젝트 관리
        shadowAnim = this.transform.GetChild(0).GetComponent<Animator>();
        shadowRand = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Think();

        if (rigid.velocity.x > 0)
            MobFlip(true);
        else if (rigid.velocity.x < 0)
            MobFlip(false);

        MobAnimator("IsWalk", rigid.velocity.magnitude > 0);
    }

    private void Think()
    {
        if (b_Hurt)
            return;

        f_AattackDelay -= Time.deltaTime;
        f_SpellDelay -= Time.deltaTime;

        moveTarget = target.transform.position + Vector3.down * 1f;
        f_Distance = Vector2.Distance(transform.position, target.transform.position);
        f_AttackDistance = Vector2.Distance(transform.position, moveTarget);

        if (f_Distance <= mobFieldOfVision && !b_IsAttack && !b_IsCast)
        {
            if (f_AattackDelay <= 0 && f_AttackDistance <= mobAttackRange && !b_IsCast)
            {
                StopCoroutine(Attack());
                StartCoroutine(Attack());
            }
            else if (f_SpellDelay <= 0 && f_AttackDistance <= mobFieldOfVision && !b_IsAttack)
            {
                StopCoroutine(Cast());
                StartCoroutine(Cast());
            }
            else if (f_AttackDistance > mobAttackRange)
                rigid.velocity = (moveTarget - this.transform.position).normalized * 0.75f;
        }
        else
            rigid.velocity = Vector2.zero;
    }

    IEnumerator Attack()
    {
        MobAnimator("IsAttack");
        b_IsAttack = true;
        f_AattackDelay = mobAttackDelay + 0.6f;
        yield return new WaitForSeconds(0.4f);

        attackArea.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        attackArea.SetActive(false);
        b_IsAttack = false;
    }

    IEnumerator Cast()
    {
        MobAnimator("IsCast");
        b_IsCast = true;
        f_SpellDelay = mobSpellDelay;
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 8; i++)
        {
            Instantiate(spellProps).transform.position = target.transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
            yield return new WaitForSeconds(0.05f);
        }

        b_IsCast = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Attack")
        {
            StopAllCoroutines();
            b_IsAttack = false;
            b_IsCast = false;
            b_Hurt = true;

            f_AattackDelay = mobAttackDelay;
            attackArea.SetActive(false);
            if (mobHealth > 1)
            {
                MobAnimator("IsHurt");
                rigid.AddForce((this.transform.position - target.transform.position).normalized * 5, ForceMode2D.Impulse);
                Invoke(nameof(NotHurt), 0.2f);
                mobHealth--;
            }
            else
            {
                rigid.velocity = Vector2.zero;
                MobAnimator("IsDeath");
            }
        }
    }

    void NotHurt() => b_Hurt = false;

    void MobFlip(bool flip)
    {
        rand.flipX = flip;
        shadowRand.flipX = flip;
        attackArea.transform.localPosition = new Vector3(flip ? 1.5f : -1.5f, 0, 0);
    }

    void MobAnimator(string str, bool bl)
    {
        anim.SetBool(str, bl);
        shadowAnim.SetBool(str, bl);
    }

    void MobAnimator(string str)
    {
        anim.SetTrigger(str);
        shadowAnim.SetTrigger(str);
    }
}
