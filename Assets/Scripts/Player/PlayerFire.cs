using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePosition;
    public float fireRate = 1.0f;
    float fireCoolTime = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 특정 시간이 지날 때 마다 총알 발사하기
        fireCoolTime -= Time.deltaTime; 

        if(fireCoolTime <= 0.0f)    // fireCoolTime의 시간이 0이 되면
        {
            GameObject bullet = Instantiate(bulletPrefab);
            Vector3 diretion = (transform.position - firePosition.transform.position).normalized;    
            // 플레이어의 위치 - 사격 방향 위치 -> 즉 플레이어가 보는 방향으로의 방향 벡터 구하기

            bullet.transform.position = firePosition.position;
            bullet.transform.rotation = Quaternion.LookRotation(diretion);  // 총알도 플레이어가 보는 방향을 바라보도록 회전

            fireCoolTime = fireRate;    // fireCoolTime 초기화 -> 즉 fireRate의 값 만큼의 총알 발사
        }
    }
}
