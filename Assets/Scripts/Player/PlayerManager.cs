using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public FixedJoystick moveJoyStick, rotationJoyStick;
    public Slider healthBar, expBar;
    public TextMeshProUGUI healthText, expText;
    public AudioClip healSound, hitSound, deathSound, levelUpSound;
    public float speed = 5.0f;
    public float rotationSpeed = 3.0f;
    public int maxHealth;
    [HideInInspector] public bool isDead = false;
    private bool isDamaged = false;
    Animator anim;
    public static PlayerManager instance;
    public int maxExp;
    [HideInInspector] public int currentExp = 0;
    [HideInInspector] public int level = 1;
    [HideInInspector] public int maxLevel = 20;
    [HideInInspector] public int currentHealth;
    AudioSource audioSource;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHealth();
        CheckExp();
    }

    void FixedUpdate()  // Update 함수에서 플레이어 이동 시 카메라가 떨리는 현상이 나타나 이를 해결하기 위해 FixedUpdate 사용
    { 
        MoveJoyStick();
        RotationJoyStick();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            TakeDamage(collision.gameObject.GetComponent<ZombieBullet>().bulletDamage);    
            audioSource.PlayOneShot(hitSound);
            Debug.Log(currentHealth);
        } 
        else if (collision.gameObject.CompareTag("Heal"))
        {
            currentHealth += 30;
            collision.gameObject.SetActive(false);
            audioSource.PlayOneShot(healSound);
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            CheckHealth();
        }
    }

    void MoveJoyStick()
    {
        float moveHorizontal = moveJoyStick.Horizontal;
        float moveVertical = moveJoyStick.Vertical;

        Vector3 diretion = new Vector3(moveHorizontal, 0.0f, moveVertical);     // 전역 공간 기준으로 방향을 설정
        // Vector3 diretion = new Vector3(moveVector.x, 0.0f, moveVector.y);    // 키보드를 이용해 움직이는 방법
        transform.position += diretion * speed * Time.deltaTime;    // 전역 공간 기준으로 이동

        Vector3 localDiretion = transform.InverseTransformDirection(diretion);  
        // 전역 공간의 방향을 지역 공간의 방향으로 변환 플레아어 캐릭터가 보는 방향에 따라 애니메이션이 실행되기 위해서

        // 현제 플레이어의 보는 방향에 따라 애니메이션을 다르게 실행
        if (localDiretion.z > 0.0f)   // 플레이어가 보는 방향 기준 앞 쪽으로 이동할 때
        {
            anim.SetBool("isWalkBackward", false); 
            anim.SetBool("isWalkForward", true);
        } 
        else if(localDiretion.z < 0.0f) // 플레이어가 보는 방향 기준 뒷 쪽으로 이동할 때
        {
            anim.SetBool("isWalkForward", false);
            anim.SetBool("isWalkBackward", true);    
        }
        else
        {
            anim.SetBool("isWalkForward", false);
            anim.SetBool("isWalkBackward", false);
        }
    }

    void RotationJoyStick() // 조이스틱으로 캐릭터 회전시키기
    {
        float rotationHorizontal = rotationJoyStick.Horizontal; // 수평 축 입력 (-1 ~ 1)
        float rotationVertical = rotationJoyStick.Vertical; // 수직 축 입력 (-1 ~ 1)

        Vector3 diretion = new Vector3(rotationHorizontal, 0.0f, rotationVertical); // 조이스틱으로 입력되어지는 값으로 방향 벡터를 생성

        if (diretion.sqrMagnitude < 0.01f) {    // 조이스틱의 입력이 없다면 회전이 없도록 함
        // diretion.sqrMagnitude = 벡터의 크기의 제곱한 값을 반환 -> 조이스틱의 입력이 없다는 것을 체크할 때는 정확한 길이가 불필요 -> 실제 길이는 루트를 씌워 계산 비용이 더 큼
        // -> 따라서 루트를 씌우지 않고 계산하는 제곱값을 이용해 조이스틱의 입력이 있는지 없는지를 체크한다.
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(diretion);  // Quaternion.LookRotation -> diretion 방향 벡터 방향으로 바라보도록 회전 값 저장

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // Quaternion.Slerp -> 두 회전 사이를 부드럽게 회전하도록 연결
        // transform.rotation 현재 회전 방향에서 targetRotation 목표 회전 방향으로 rotationSpeed * Time.deltaTime의 속도만큼 부드럽게 회전
    } 

    public void TakeDamage(int damage) // 플레이어 체력 구현하기
    {
        if (isDead)
        {
            return;
        }

        if (!isDamaged)
        {
            isDamaged = true;
            currentHealth -= damage;
            audioSource.PlayOneShot(hitSound);
            CheckHealth();

            StartCoroutine(DelayDamage());
        } 
        else
        {
            return;
        }

        if(currentHealth <= 0)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    public void CheckHealth()  
     // 플레이어 체력 바 만들기
    {
        healthBar.value = (float)currentHealth / maxHealth;
        // Mathf.Lerp = 선형보간 -> 두 점 a, b 사이의 값(c)을 구하기 위해 두 점을 연결한 직선을 만들어 사이 값을 계산하는 방법이다.
        healthText.text = currentHealth + " / " + maxHealth;
    }

    public void CheckExp()
    {
        expBar.value = (float)currentExp / (float)maxExp;
        expText.text = currentExp + " / " + maxExp;
        if(currentExp >= maxExp)
        {
            if(level > maxLevel)
            {
                level = maxLevel;
                return;
            }
            else
            {
                LevelUp();
            }
            
        }
    }

    public void LevelUp()
    {
        level++;
        maxExp = currentExp + (1 * (level * level));
        maxHealth += 5;
        speed += 0.125f;
        audioSource.PlayOneShot(levelUpSound);
        Debug.Log("속도 : " + speed + " 최대 체력 : " + maxHealth + " 최대 경험치 : " + maxExp);
    }

    IEnumerator Die()
    {
        speed = 0.0f;
        rotationSpeed = 0.0f;

        anim.SetBool("Die", true);
        audioSource.PlayOneShot(deathSound);

        yield return new WaitForSeconds(2.5f);

        gameObject.SetActive(false);    // 플레이어를 Destroy 하면 적과 카메라가 쫓아갈 플레이어가 없어지므로 SetActive로 안보이게만 만들기
        GameManager.instance.EndGame();
    }

    IEnumerator DelayDamage()   // 3초 동안 무적
    {
        yield return new WaitForSeconds(3.0f);

        isDamaged = false;
    }
}


