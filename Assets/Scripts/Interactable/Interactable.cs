using UnityEngine;

namespace Interactable
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public GameObject IconInteract;
        
        public void Interact()
        {
            Debug.Log("Estoy interactuando con el dragon");
        }

        public void ShowCanInteract(bool show)
        {
            IconInteract.SetActive(show);
        }
    }
}
