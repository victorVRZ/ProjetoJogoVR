using UnityEngine;

public class ArrowNockDetector : MonoBehaviour
{
    public BowLogic bow; // Arraste o objeto que tem o script BowLogic aqui

    void Start()
    {
        if (bow == null)
        {
            bow = GetComponentInParent<BowLogic>();
        }

        if (bow == null)
        {
            Debug.LogError("ArrowNockDetector: BowLogic não encontrado! Arraste o Arco para o campo 'Bow' no Inspector.");
        }

        // Verifica se o objeto tem um collider configurado como Trigger
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogWarning("ArrowNockDetector: O objeto " + gameObject.name + " precisa de um Collider com 'Is Trigger' marcado!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("ArrowNockDetector: Algo entrou no trigger: " + other.gameObject.name + " com tag: " + other.tag);

        if (other.CompareTag("Arrow")) 
        {
            if (bow != null)
            {
                bow.NockManual(other.gameObject);
            }
            else
            {
                Debug.LogError("ArrowNockDetector: Referência ao BowLogic está nula!");
            }
        }
    }
}