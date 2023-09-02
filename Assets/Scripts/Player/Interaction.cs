using UnityEngine;

namespace Player
{
    public class Interaction : MonoBehaviour
    {
        [SerializeField]
        private Transform _colliderOffset;
    
        [SerializeField]
        private LayerMask _interactableLayer;

        private IInteractable _interactable;

        private float _checkDistance = 0.6f;

        public bool IsInteracting { get; private set; }
        
        private void Update()
        {
            CanInteract();
            Debug.Log("Puedo Interactuar? " + CanInteract());
        }

        public void Interact()
        {
            StopInteracting();
            
            if (CanInteract())
            {
                _interactable?.Interact();
                IsInteracting = true;         
            }
        }

        public bool CanInteract()
        {
            _interactable?.ShowCanInteract(false);
            
            Vector3 origin = transform.position + _colliderOffset.position;
            
            RaycastHit hit;
            
            if (Physics.Raycast(origin, transform.forward, out hit, _checkDistance, _interactableLayer))
            {
                _interactable = hit.collider.GetComponent<IInteractable>();
            }
            else
            {
                _interactable = null;
            }
            
            if (_interactable != null)
            {
                _interactable?.ShowCanInteract(true);
                return true;
            }  
            
            origin = transform.position + _colliderOffset.position;
            
            if (Physics.Raycast(origin, transform.forward, out hit, _checkDistance, _interactableLayer))
            {
                _interactable = hit.collider.GetComponent<IInteractable>();
            }
            else
            {
                _interactable = null;
            }

            if (_interactable != null)
            {
                _interactable?.ShowCanInteract(true);
                return true;
            }  
            
            return false;
        }

        private void StopInteracting()
        {
            if (_interactable == null) return;

            _interactable.ShowCanInteract(false);
            _interactable = null;
            IsInteracting = false;
        }
    }
}
