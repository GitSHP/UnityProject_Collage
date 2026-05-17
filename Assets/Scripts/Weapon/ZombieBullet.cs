using UnityEngine;

public class ZombieBullet : Bullet
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        bulletLifeTime = 5.0f;
        GameObject player = GameObject.Find("Player");
        if (player)
        {
            diretion = -(transform.position - player.transform.position).normalized;
        }
        Destroy(gameObject, bulletLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += diretion * bulletSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().TakeDamage(bulletDamage);
            Destroy(gameObject);
        } 
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
