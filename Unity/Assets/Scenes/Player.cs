using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public VariableJoystick joy;
    public GameObject attackArea;
    public float speed;
    public float damage;
    public float attackSpeed;

    private Animator animator;
    private SpriteRenderer rand;
    private Rigidbody2D rigid;

    private Animator shadowAnimator;
    private SpriteRenderer shadowRand;

    private Vector2 dir;

    private bool b_Dash;
    private bool b_Slide;
    private bool b_Attack;
    private bool b_Hurt;

    private float f_Time;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rand = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        // 자식 오브젝트 관리
        shadowAnimator = this.transform.GetChild(0).GetComponent<Animator>();
        shadowRand = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }


    private void Update()
    {
        EvasionCooltime();
        PlayerMove();
    }

    void PlayerMove()
    {
        if (b_Attack || b_Hurt)
            return;

        dir.x = joy.Horizontal;
        dir.y = joy.Vertical;

        if (Input.GetKey(KeyCode.A))
        {
            dir.x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir.x = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            dir.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir.y = -1;
        }

        if (dir.x < 0)
        {
            PlayerFlip(true);
            attackArea.transform.localPosition = new Vector2(-0.5f, 0);
        }
        else if (dir.x > 0)
        {
            PlayerFlip(false);
            attackArea.transform.localPosition = new Vector2(0.5f, 0);
        }

        dir.Normalize();
        PlayerAnimator("IsMoving", dir.magnitude > 0);

        rigid.velocity = b_Slide || b_Dash ? speed * dir * 1.5f : speed * dir;
    }

    public void Evasion()
    {
        if (dir.x == 0 && dir.y == 0)
            return;

        if (!b_Dash)
        {
            b_Dash = true;
            PlayerAnimator("IsDash", b_Dash);
        }
        else if (!b_Slide && b_Dash)
        {
            f_Time = 0;
            b_Slide = true;
            PlayerAnimator("IsSlide", b_Slide);
        }
    }

    void EvasionCooltime()
    {
        if (b_Slide || b_Dash)
        {
            dir.x = rand.flipX ? -1 : 1;
            f_Time += Time.deltaTime;
            if (f_Time > 0.5f)
            {
                b_Dash = false;
                b_Slide = false;
                f_Time = 0;
                PlayerAnimator("IsDash", b_Dash);
                PlayerAnimator("IsSlide", b_Slide);
            }
        }
    }

    public void AttackCoroutine()
    {
        if (b_Attack || b_Hurt)
            return;

        StopCoroutine(Attack());
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        PlayerAnimator("IsAttack");
        rigid.velocity = Vector2.zero;
        b_Attack = true;
        yield return new WaitForSeconds(0.5f);

        attackArea.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        b_Attack = false;
        attackArea.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Monster")
        {
            if (b_Slide || b_Hurt)
                return;

            StopAllCoroutines();
            b_Hurt = true;
            b_Attack = false;
            PlayerAnimator("IsHurt");
            rigid.AddForce((this.transform.position - collision.transform.position).normalized * 5, ForceMode2D.Impulse);
            Invoke("NotHurt", 0.3f);
        }
    }

    void NotHurt()
    {
        b_Hurt = false;
    }

    void PlayerFlip(bool flip)
    {
        rand.flipX = flip;
        shadowRand.flipX = flip;
    }

    void PlayerAnimator(string str, bool bl)
    {
        animator.SetBool(str, bl);
        shadowAnimator.SetBool(str, bl);
    }

    void PlayerAnimator(string str)
    {
        animator.SetTrigger(str);
        shadowAnimator.SetTrigger(str);
    }
}
