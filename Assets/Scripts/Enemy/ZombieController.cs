using UnityEngine;
using System.Collections;

public class ZombieController : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxHealth = 100 + GameManager.instance.currentMin * 25;
        speed = speed + GameManager.instance.currentMin * 0.20f;
        currentHealth = maxHealth;
        player = GameObject.Find("Player");
        currentState = EnemyState.Chase;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {   
        // 제미나이 이용
        // 플레이어와 특정 거리 이상 가까워지면 잠시 동작을 멈추고 원거리 공격만 하도록 하는 AI
        switch (currentState)   // 현재 적 상태를 확인
        {
            case EnemyState.Chase:  // 현재 적이 Chase 상태라면
                ChasePlayer();  // 적이 플레이어를 추적하면서
                break;

            case EnemyState.Attack: // 현재 적이 Attack 상태이면
                                    // 공격 중에는 플레이어를 추적하거나 거리 값을 계산하지 않음
                break;
        }
    }

    void ChasePlayer()
    {
        // https://withchan.tistory.com/42 - 플레이어 캐릭터를 추적하는 적 AI 만드는 법
        
        if (player)
        {
            Vector3 targetPosition = player.transform.position; // 적이 플레이어를 향해 움직일 때마다 둥둥 떠다니는 현상 발견
            targetPosition.y = transform.position.y;    // 이는 플레이어의 피벗 값이 Y = 0 값보다 높기 때문에 발생하는 현상으로 보임
            // 이를 해결하기 위해 X값과 Z값은 그대로 두고 Y값만 0으로 고정하여 적이 플레이어을 향해 갈 때 둥둥 떠다니지 못하도록 수정
            // 제미나이

            diretion = targetPosition - transform.position;  // 플레이어의 위치 값에서 자신의 위치 값을 빼서 방향 벡터를 구한다.
            rotation = Quaternion.LookRotation(diretion);
            gameObject.transform.rotation = rotation;
            // Quaternion.LookRotation(diretion) diretion 방향으로 바라보도록 회전하고 그 값을 rotation 변수에 저장한다.

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            // Vector3.MoveTowards = 자신의 위치(transform.position)에서 플레이어의 위치(player.position)까지 speed * Time.deltaTime의 속도로
            // 자신의 위치 값을 변경한다 -> 이동한다.

            currentState = EnemyState.Chase;
            
            Vector3 localDiretion = transform.InverseTransformDirection(diretion);  
            // 전역 공간의 방향을 지역 공간의 방향으로 변환 플레아어 캐릭터가 보는 방향에 따라 애니메이션이 실행되기 위해서 - 제미나이
            
            // 현재 게임 오브젝트가 보는 방향에 따라 애니메이션을 다르게 실행 - 제미나이
            if (localDiretion.z > 0.0f)   // 적이 보는 방향 기준 앞 쪽으로 이동할 때
            {
                anim.SetBool("isMove", true);
            } 
            else
            {
                anim.SetBool("isMove", false);
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("충돌 감지");
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DoAttack());
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isAttacking)
            {
                StartCoroutine(DoAttack());
            }
        }
    }

    IEnumerator DoAttack()  // 코루틴 사용을 위한 열거자 인터페이스 - 제미나이 사용
    {
        isAttacking = true; // 현재 적 상태를 공격 중으로 변경
        currentState = EnemyState.Attack;   // 현재 적 상태를 공격 중으로 변경

        anim.SetBool("isAttack", true);

        player.GetComponent<PlayerManager>().TakeDamage(attackDamage);

        yield return new WaitForSeconds(2.6f);    // 애니메이션의 시간만큼 함수 실행 일시 정지

        anim.SetBool("isAttack", false);

        isAttacking = false;    // 애니메이션이 종료되면 현재 적 상태를 공격 중이 아님으로 변경
        currentState = EnemyState.Chase;    // 현재 적 상태를 추적 상태로 변경
    }
}
