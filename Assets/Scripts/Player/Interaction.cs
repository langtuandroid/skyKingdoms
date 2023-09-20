using UnityEngine;

namespace Player
{
    public class Interaction : MonoBehaviour
    {
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

            float angleStep = 10.0f; 
            float visionAngle = 60.0f; 

            for (float angle = -visionAngle / 2; angle < visionAngle / 2; angle += angleStep)
            {
                Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
                Vector3 origin = transform.position; // Origen corregido

                RaycastHit hit;
                
                // Incrementada la duración del rayo dibujado para facilitar la depuración
                Debug.DrawRay(origin, dir * _checkDistance, Color.red, 1.0f);

                if (Physics.Raycast(origin, dir, out hit, _checkDistance, _interactableLayer))
                {
                    _interactable = hit.collider.GetComponent<IInteractable>();
                    if (_interactable != null)
                    {
                        _interactable.ShowCanInteract(true);
                        return true;
                    }
                }
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
