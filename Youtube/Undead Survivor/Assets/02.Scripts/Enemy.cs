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
    //WaitForFixedUpdate wait; // 코루틴

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
        if (!GameManager.instance.isLive) // if 문을 2개 사용해도 됌
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit")) // GetCurrentAnimatorStateInfo 현재 상태 정보를 가져옴
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero; // <- 물리 속도 제거
    }

    private void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable() // 스크립트가 활성화 될 때 호출되는 이벤트 함수
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true; // 비활성화 되어 false 되어 있는 것을 true로 바꿈
        coll.enabled = true; // Collider 다시 활성화
        rigid.simulated = true; // Rigid 다시 활성화
        spriter.sortingOrder = 2; // Order in Layer 를 다시 2로 만들기
        anim.SetBool("Dead", false); // Dead Animation 의 Dead 를 다시 false
        health = maxHealth; // 체력을 초기화
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
        if (!collision.CompareTag("Bullet") || !isLive) // !isLive 사망 로직이 연달아 실행되는 것을 방지
            return;

        health -= collision.GetComponent<Bullet>().damage;
        //StartCoroutine("KnockBack");
        StartCoroutine(KnockBack()); //위와 실행은 같음

        if (health > 0)
        {
            // ... Live, Hit Action
            anim.SetTrigger("Hit"); // Animation 에서 Hit Trigger 활성/비활성
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            isLive = false; // 죽었을 시 Hit 와 KnockBack 함수를 비활성화 하기 위한 코드
            coll.enabled = false; // Collider 비활성화
            rigid.simulated = false; // Rigid 비활성화
            spriter.sortingOrder = 1; // Order in Layer 를 1로 만들어 플레이어 뒤로 보냄 가리지않게.
            anim.SetBool("Dead", true); // Dead Animation 실행
            // ... Dead()를 한다면 애니메이션은 실행되지 않고 바로 비활성화 된다
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }

    }
    // Coroutine - 생명주기와 비동기처럼 실행되는 함수
    IEnumerator KnockBack() // KnockBack 함수
    {
        yield return new WaitForFixedUpdate(); // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; // Player의 반대방향
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); // 크기까지 1로 만들고 즉발적 힘을 가함

        //yield return null; // 1프레임 쉬기

        //yield return new WaitForSeconds(2f); // 2초 쉬기
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
