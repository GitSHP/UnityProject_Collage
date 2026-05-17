using UnityEngine;

public class ARFire : Gun
{
    public AudioClip fireSound;
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gunName = "AssaultRifle";
        fireRate = 1.0f;
        fireCoolTime = fireRate;
        firePosition = transform.root.Find("firePosition");
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerManager.instance.isDead == false)
        {
            // 특정 시간이 지날 때 마다 총알 발사하기 - 챗GPT 사용 -> 코루틴 방법도 있지만 너무 복잡함..
            fireCoolTime -= Time.deltaTime; 

            if(fireCoolTime <= 0.0f)    // fireRate의 시간이 0이 되면 -> 총알 발사 속도 조절
            {
                GameObject bullet = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);

                bullet.transform.rotation = firePosition.rotation;  // 총알도 플레이어가 보는 방향을 바라보도록 회전

                fireCoolTime = fireRate - (PlayerManager.instance.level * 0.025f);    // fireCoolTime 초기화 -> 즉 fireRate의 값 만큼의 총알 발사
                audioSource.PlayOneShot(fireSound);
            }
        }
        else
        {
            return;
        }
    }
}
