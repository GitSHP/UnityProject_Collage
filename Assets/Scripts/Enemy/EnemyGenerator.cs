using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject normalZombiePrefab; // 스폰될 3가지의 적 종류들
    public GameObject fastZombiePrefab;
    public GameObject rangeZombiePrefab;
    public GameObject bossZombiePrefab;
    
    public BoxCollider mapArea; // 맵 범위를 결정할 BoxCollider mapArea 변수
    float mapAreahalf_x;
    float mapAreahalf_z;
    public Camera cam;  // 카메라 범위를 확인할 Camera cam 변수
    public AudioClip enemyCommingSound, bossCommingSound;
    AudioSource audioSource;
    GameObject enemy;
    float spawnTime; // 적 스폰 시간
    float spawnMinTime = 1.5f;  // 최소 적 스폰 시간
    float spawnMaxTime = 3.0f;  // 최대 적 스폰 시간
    float nextSpawnTime = 0.0f;   // 다음 적이 스폰될 시간
    int enemyChance;    // 다른 종류의 적이 스폰될 확률
    int maxEnemyCount = 100;    // 최대 적의 수
    bool isEnemyComming = false;
    bool isBossSpawned = false;
    public int currentEnemyCount = 0;  // 현재 적의 수
    public static EnemyGenerator instance = null;
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
        mapAreahalf_x = mapArea.size.x / 2;
        mapAreahalf_z = mapArea.size.z / 2;

        Debug.Log("맵 절반 x : " + mapAreahalf_x);
        Debug.Log("맵 절반 z : " + mapAreahalf_z);

        audioSource = GetComponent<AudioSource>();
        spawnTime = 3;  // 시작하자마자 바로 적이 스폰되는 것을 방지
    }

    // Update is called once per frame
    void Update()
    {
        nextSpawnTime += Time.deltaTime;
        if (nextSpawnTime > spawnTime)
        {
            if(currentEnemyCount < maxEnemyCount)
            {
                SpawnEnemy();
            }
            nextSpawnTime = 0;
            spawnTime = Random.Range(spawnMinTime, spawnMaxTime);
            Debug.Log(currentEnemyCount);
        }

        if(GameManager.instance.currentTime >= 120.0f && !isEnemyComming)
        // Time.deltaTime은 부동 소수점으로 계산하는 시간이기 때문에 오차가 발생해 정확히 3.0f가 일치하는 순간이 일어나지 않음.
        // int로 바꾼다고 해도 소수점이기 때문에 3초인 경우가 매우 많음. -> 따라서 isEnemyComming 을 이용해 해당 시간대에 한 번만 실행되도록 한다. - 제미나이
        {
            isEnemyComming = true;  // 한 번 적 스폰 이후 다음 프레임에서 이 적을 스폰하지 못하도록 방지

            GameManager.instance.ShowEnemyCommingPanel();
            
            audioSource.PlayOneShot(enemyCommingSound);
            for(int i = 0; i < 30; i++)
            {
                SpawnEnemy();
            }
        }

        if(GameManager.instance.currentTime >= 240.0f && !isBossSpawned)
        {
            isBossSpawned = true;

            StartCoroutine(GameManager.instance.BossAlert());

            audioSource.PlayOneShot(bossCommingSound);

            GameObject boss = Instantiate(bossZombiePrefab);
            boss.transform.position = SetSpawnPosition();
        }

    }
    
    Vector3 SetSpawnPosition()  // 적의 스폰 위치를 결정하는 함수 + 맵 안에서만 스폰하되 플레이어 카메라 밖에서만 스폰되도록 함
    {   // 제미나이 이용 + 아래 링크 참조
        // https://velog.io/@gkswh4860/Unity-%ED%8A%B9%EC%A0%95-%EB%B2%94%EC%9C%84-%EB%82%B4%EC%97%90%EC%84%9C-%EB%9E%9C%EB%8D%A4%ED%95%9C-%EC%9C%84%EC%B9%98%EC%97%90-%EC%98%A4%EB%B8%8C%EC%A0%9D%ED%8A%B8-%EC%8A%A4%ED%8F%B0%ED%95%98%EA%B8%B0



        float spawnArea_x = Random.Range(-mapAreahalf_x, mapAreahalf_x);   // 맵의 오른쪽 끝에서 왼쪽 끝 사이의 값 들 중 랜덤하게 값을 선택
        float spawnArea_z = Random.Range(-mapAreahalf_z, mapAreahalf_z);   // 맵의 위 쪽 끝에서 아래 쪽 끝 사이의 값 들 중 랜덤하게 값을 선택
        // -> 즉 맵 범위 안에서 랜덤한 위치의 값을 랜덤하게 선택

        Debug.Log(mapArea.bounds.min.z + " & " + mapArea.bounds.max.z);

        Vector3 spawnPosition = new Vector3(spawnArea_x, 0, spawnArea_z);   // 랜덤하게 선택된 값을 스폰할 좌표로 선택

        Vector3 viewPoint = cam.WorldToViewportPoint(spawnPosition);   
        // spawnPosition 좌표가 카메라의 밖에 있는지 안에 있는지 판별하기 위한 viewPoint 변수
        // cam.WorldToViewportPoint(spawnPosition) -> 월드 좌표(spawnPosition)를 뷰포트(Vector3 Viewport) 좌표로 변환해주는 함수
        // 뷰보트 좌표 = 스크린 좌표를 정규화한 좌표

        if(viewPoint.z > 0 && viewPoint.x > 0 && viewPoint.x < 1 && viewPoint.y > 0 && viewPoint.y < 1)
        // spawnPosition의 좌표 값이 화면 안에 존재한다면
        {
            return spawnPosition;  // 스폰할 위치 값을 다시 선택
        } 
        else
        {
            return spawnPosition;   // 화면 밖, 맵 안에 존재한다면 그 위치 값을 리턴
        }
    }

    public void SpawnEnemy()
    {
        enemyChance = Random.Range(0, 10);
        if (enemyChance >= 0 && enemyChance < 6)
        {
            enemy = Instantiate(normalZombiePrefab);
            enemy.transform.position = SetSpawnPosition();
            currentEnemyCount++;
        }
            else if (enemyChance >= 6 && enemyChance < 8)
        {
            enemy = Instantiate(fastZombiePrefab);
            enemy.transform.position = SetSpawnPosition();
            currentEnemyCount++;
        }
        else
        {
            enemy = Instantiate(rangeZombiePrefab);
            enemy.transform.position = SetSpawnPosition();
            currentEnemyCount++;
        }
    }
}
