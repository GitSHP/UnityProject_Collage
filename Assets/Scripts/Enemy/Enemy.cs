using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour // 적의 공통 기능을 정의하는 클래스
{
    protected GameObject player;  // 플레이어의 위치 값을 찾기 위해 사용하는 게임 오브젝트 변수
    protected Animator anim;   // 애니메이션을 저장할 변수
    protected EnemyState currentState;
    public GameObject healItemFactory;
    public AudioClip hitSound, deathSound, bossHitSound;
    protected AudioSource audioSource;
    public float speed;         // 최대 체력, 속도, 공격력은 스크립트에서 조절하지않고 유니티 인스펙터에서 조절
    public int attackDamage;
    public int maxHealth;
    protected float originSpeed;
    [HideInInspector]
    public int currentHealth;
    protected bool isDead = false;
    protected bool isAttacking;   // 현재 공격 중인지 아닌지를 확인하기 위한 변수
    protected Vector3 diretion;   // 플레이어의 방향 벡터값을 저장하는 변수
    protected Quaternion rotation;    // 플레이어의 방향으로 회전하도록 회전 값을 저장하는 변수
    protected enum EnemyState // 적의 상태를 저장하는 enum 클래스
    {        
        Chase, Attack 
    }

    void Awake()
    {
        originSpeed = speed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)  
    {
        Debug.Log("좀비 피격");
        if (isDead) // 적이 이미 죽고있는 상황에서 총알을 맞으면 여러 번 죽는 것처럼 인식하는 상황 발생 - 이를 해결하기 위한 isDead - 제미나이
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("현재" + gameObject.name + "의 체력 : " + currentHealth);

        if(currentHealth <= 0)
        {        
            speed = 0.0f;
            isDead = true;
            StartCoroutine(Die());
        }
        else
        {
            StopCoroutine(DelayMove());
            StartCoroutine(DelayMove());
        }
    }

    public IEnumerator DelayMove() // 적의 움직임을 멈추도록 하는 함수
    {
        speed = 0.0f;   
        // 적이 DelayMove 코루틴을 호출한 상태에서 한번 더 피격 당하면 speed가 0으로 고정되는 문제 발생 - tempSpeed = speed하면 DelayMove 하는동안 Speed = 0 이고
        // 이를 코루틴 종료 시 speed = tempSpeed에 의해 speed가 영구히 0으로 바뀌는 현상 발생
        // 따라서 이를 해결하기 위해 tempSpeed가 아닌 원래 속도를 저장하는 변수를 사용해서 이를 해결한다. - 제미나이

        anim.SetBool("Hit",true);

        audioSource.PlayOneShot(hitSound);        

        yield return new WaitForSeconds(2.6f);

        anim.SetBool("Hit",false);

        speed = originSpeed;
    }

    public IEnumerator Die()
    {
        currentState = EnemyState.Attack; // 적이 죽으면 아무것도 하지 못하게 만듬 -> 현재 상태가 attack일 때는 특별히 하는 일이 없음

        audioSource.PlayOneShot(deathSound);

        anim.SetBool("Die", true);

        yield return new WaitForSeconds(2.9f);

        EnemyGenerator.instance.currentEnemyCount--;
        PlayerManager.instance.currentExp++;
        GameManager.instance.killCount++;

        Debug.Log("적 없어짐@@@");

        int itemChance = Random.Range(0,10);
        if(itemChance < 3)
        {
            GameObject healItem = Instantiate(healItemFactory);
            healItem.transform.position = new Vector3(gameObject.transform.position.x, 1, gameObject.transform.position.z);
        }

        Destroy(gameObject);
    }
}
