using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveRange = 3f;

    private Vector3 startPosition;
    private float direction = 1f;
    private TargetSpawner spawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }
    public void SetSpawner(TargetSpawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    // Update is called once per frame
    void Update()
    {
        // Calcula o offset atual em relação à posição inicial
        float currentOffset = transform.position.x - startPosition.x;

        // Inverte a direção ao atingir o limite
        if (currentOffset >= moveRange)
            direction = -1f;
        else if (currentOffset <= -moveRange)
            direction = 1f;

        // Move o target
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow")) 
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            if (spawner != null)
                spawner.OnTargetDestroyed();
        }
    }
}
