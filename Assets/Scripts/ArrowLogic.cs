using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Bow currentBow; // referência pode ser setada ao encaixar

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    public void OnGrabbed()
    {
        // Se a flecha estava nockada, avisa o arco
        if (currentBow != null)
        {
            currentBow.ForceReleaseArrow();
            currentBow = null;
        }
    }

    // Opcional: pode limpar currentBow quando a flecha é solta longe do arco
}