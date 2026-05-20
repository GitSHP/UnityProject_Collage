using System;
using System.Collections;
using UnityEngine;

public class RangeZombieController : Enemy
{
    public GameObject zombieBulletPrefab;   // 적이 사용할 원거리 공격 총알 프리팹
    public GameObject firePosition; // 적이 발사한 총알이 생성될 위치
    float attackDistance = 8.0f;    // 플레이어의 거리가 특정 거리보다 짧아졌을 때 공격하도록 특정 거리 값을 저장하는 변수
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentState = EnemyState.Chase; // 현재 적 상태를 저장하는 변수
        isAttacking = false;
        maxHealth = 80 + GameManager.instance.currentMin * 15;  
        speed = speed + GameManager.instance.currentMin * 0.10f;  
        currentHealth = maxHealth;
        player = GameObject.Find("Player");
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
                CheckPlayerDistanceAndAttack(); // 적과 플레이어 사이의 거리를 계산, 거리가 attackDistance보다 짧아졌을 때 적이 플레이어를 공격
                break;

            case EnemyState.Attack: // 현재 적이 Attack 상태이면
                                    // 공격 중에는 플레이어를 추적하거나 거리 값을 계산하지 않음
                break;
        }
            
    }
    void ChasePlayer()
    {
        if (player)
        {
            currentState = EnemyState.Chase;

            anim.SetBool("isIdel", false);

            Vector3 targetPosition = player.transform.position; // 적이 플레이어를 향해 움직일 때마다 둥둥 떠다니는 현상 발견
            targetPosition.y = transform.position.y;    // 이는 플레이어의 피벗 값이 Y = 0 값보다 높기 때문에 발생하는 현상으로 보임
            // 이를 해결하기 위해 X값과 Z값은 그대로 두고 Y값만 0으로 고정하여 적이 플레이어을 향해 갈 때 둥둥 떠다니지 못하도록 수정

            diretion = targetPosition - transform.position;  // 플레이어의 위치 값에서 자신의 위치 값을 빼서 방향 벡터를 구한다.
            rotation = Quaternion.LookRotation(diretion);
            gameObject.transform.rotation = rotation;
            // Quaternion.LookRotation(diretion) diretion 방향으로 바라보도록 회전하고 그 값을 rotation 변수에 저장한다.

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            // Vector3.MoveTowards = 자신의 위치(transform.position)에서 플레이어의 위치(player.position)까지 speed * Time.deltaTime의 속도로
            // 자신의 위치 값을 변경한다 -> 이동한다.

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

    void CheckPlayerDistanceAndAttack() // 적과 플레이어 사이의 거리를 계산, 거리가 attackDistance보다 짧아졌을 때 적이 플레이어를 공격하는 함수
    {
        // 적과 플레이어 사이의 거리를 계산하는 방법
        float distanceToPlayer;
        try
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        } 
        catch(NullReferenceException)
        {
            return;
        }
        
        // 플레이어와 적 사이의 거리를 계산해서 변수에 저장

        if (!isAttacking)   // 제미나이 이용 // 만약 적이 공격 중이 아니라면
        {
            if(distanceToPlayer < attackDistance)   // 플레이어와 적 사이의 거리를 계산해서 그 거리가 attackDistance보다 짧다면
            {
                currentState = EnemyState.Attack;   // 현재 적 상태를 공격 상태로 변경
                StartCoroutine(DoAttack());         // DoAttack() 함수를 코루틴으로 실행
            } 
            else
            {
                currentState = EnemyState.Chase;    // 플레이어와 적 사이의 거리를 계산해서 그 거리가 attackDistance보다 멀다면 계속 추적
            }
        }
    }
    
    IEnumerator DoAttack()
    {
        anim.SetBool("isMove", false);
        anim.SetBool("isIdel", true);   // 공격하기 위해 멈춰 있는 동안은 움직이는 애니메이션이 아닌 가만히 서 있는 애니메이션이 나오도록 함

        isAttacking = true; // 현재 적 상태를 공격 중으로 변경
        currentState = EnemyState.Attack;   // 현재 적 상태를 공격 중으로 변경

        yield return new WaitForSeconds(1f);    
        // 적이 플레이어와 일정 거리 이상 짧아졌을 때 바로 공격해 피하기 어렵기 때문에 공격 상태가 돼면 바로 공격하지 못하고 잠시 멈춰있도록 함
        
        diretion = player.transform.position - transform.position;
        diretion.y = 0;
        // 적이 플레이어를 원거리 공격할 때, 추적할 때 x축을 기준으로 회전하는 현상 발생
        // 이는 플레이어와 적이 같은 y축 상에 존재하지 않기 때문에 플레이어가 높은 곳, 낮은 곳에 위치한다면 방향 벡터가 그곳을 향하기 때문에
        // 시간이 지날수록, 원거리 공격을 할 수록 x축을 기준으로 회전하게 된다.
        // 이를 막기 위해서 y축은 무시하고 계산하도록 한다. - 제미나이 

        rotation = Quaternion.LookRotation(diretion);
        gameObject.transform.rotation = rotation;

        GameObject zombieBullet = Instantiate(zombieBulletPrefab);  // 적의 총알을 생성
        zombieBullet.transform.position = firePosition.transform.position;  // 그 총알이 firePosition 위치로 이동

        anim.SetBool("isIdel", false);

        anim.SetBool("isAttack", true);   // 적이 가지고 있는 공격 애니메이션을 실행

        yield return new WaitForSeconds(2.6f);    // attackDuration -> 애니메이션의 시간만큼 함수 실행 일시 정지

        isAttacking = false;    // 애니메이션이 종료되면 현재 적 상태를 공격 중이 아님으로 변경
        currentState = EnemyState.Chase;    // 현재 적 상태를 추적 상태로 변경

        anim.SetBool("isAttack", false);
    }
}
