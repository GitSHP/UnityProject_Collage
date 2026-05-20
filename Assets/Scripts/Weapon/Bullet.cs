using UnityEngine;

public class Bullet : MonoBehaviour // 총알의 기능을 정의하는 클래스
{
    public float bulletSpeed;     // 총알의 속도를 지정
    public int bulletDamage;    // 총알의 데미지를 지정
    public float bulletLifeTime;    // 이 시간이 지나면 총알이 자동으로 없어짐 -> 사거리를 제한할 수 있음 - 제미나이
    protected Vector3 diretion;     // 총알이 날아갈 방향을 지정

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletDamage = 25 + PlayerManager.instance.level * 5;
        Destroy(gameObject, bulletLifeTime);    // 총알이 일정 시간이 지나면 자동으로 삭제
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * bulletSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
            enemy.TakeDamage(bulletDamage);
            // 적에게 데미지 주기 위해 적의 스크립트 컴포넌트가 상속받고 있는 클래스(GetComponentInParent를 이용)를 찾아서 그곳에서 damage와 연결해 적의 체력 낮추기

            Destroy(gameObject);
        } 
        else if (other.gameObject.CompareTag("Boss"))
        {
            BossZombieController boss = other.gameObject.GetComponent<BossZombieController>();
            boss.TakeDamage(bulletDamage);
            // 적에게 데미지 주기 위해 적의 스크립트 컴포넌트가 상속받고 있는 클래스(GetComponentInParent를 이용)를 찾아서 그곳에서 damage와 연결해 적의 체력 낮추기

            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Temp"))
        {
            Debug.Log("충돌 감지");
            Destroy(gameObject);
        }
    }
}
