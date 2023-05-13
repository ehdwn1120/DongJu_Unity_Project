using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    //WaitForFixedUpdate wait; // �ڷ�ƾ

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        //wait = GetComponent<WaitForFixedUpdate>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.isLive) // if ���� 2�� ����ص� ��
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) // GetCurrentAnimatorStateInfo ���� ���� ������ ������
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero; // <- ���� �ӵ� ����
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable() // ��ũ��Ʈ�� Ȱ��ȭ �� �� ȣ��Ǵ� �̺�Ʈ �Լ�
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true; // ��Ȱ��ȭ �Ǿ� false �Ǿ� �ִ� ���� true�� �ٲ�
        coll.enabled = true; // Collider �ٽ� Ȱ��ȭ
        rigid.simulated = true; // Rigid �ٽ� Ȱ��ȭ
        spriter.sortingOrder = 2; // Order in Layer �� �ٽ� 2�� �����
        anim.SetBool("Dead", false); // Dead Animation �� Dead �� �ٽ� false
        health = maxHealth; // ü���� �ʱ�ȭ
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive) // !isLive ��� ������ ���޾� ����Ǵ� ���� ����
            return;

        health -= collision.GetComponent<Bullet>().damage;
        //StartCoroutine("KnockBack");
        StartCoroutine(KnockBack()); //���� ������ ����

        if (health > 0)
        {
            // ... Live, Hit Action
            anim.SetTrigger("Hit"); // Animation ���� Hit Trigger Ȱ��/��Ȱ��
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false; // �׾��� �� Hit �� KnockBack �Լ��� ��Ȱ��ȭ �ϱ� ���� �ڵ�
            coll.enabled = false; // Collider ��Ȱ��ȭ
            rigid.simulated = false; // Rigid ��Ȱ��ȭ
            spriter.sortingOrder = 1; // Order in Layer �� 1�� ����� �÷��̾� �ڷ� ���� �������ʰ�.
            anim.SetBool("Dead", true); // Dead Animation ����
            // ... Dead()�� �Ѵٸ� �ִϸ��̼��� ������� �ʰ� �ٷ� ��Ȱ��ȭ �ȴ�
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }

    }
    // Coroutine - �����ֱ�� �񵿱�ó�� ����Ǵ� �Լ�
    IEnumerator KnockBack() // KnockBack �Լ�
    {
        yield return new WaitForFixedUpdate(); // ���� �ϳ��� ���� ������ ������
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; // Player�� �ݴ����
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); // ũ����� 1�� ����� ����� ���� ����

        //yield return null; // 1������ ����

        //yield return new WaitForSeconds(2f); // 2�� ����
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
