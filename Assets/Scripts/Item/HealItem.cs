using UnityEngine;

public class HealItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 15.0f); // 체력 회복 아이템 스폰 이후 15초 지나면 사라지도록 함
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 30, 0 ) * Time.deltaTime);  // 제자리에서 회전
    }
}
