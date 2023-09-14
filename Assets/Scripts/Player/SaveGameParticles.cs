using UnityEngine;
namespace Player
{
    public class SaveGameParticles : MonoBehaviour
    {
        private float startY = 2f;
        private float endY = 1f;
        private float lerpSpeed = 1f;
        private bool _playerHasEnter;

        private void Start()
        {
            startY = transform.position.y;
        }
        
        private void Update()
        {
           /* var position = transform.position;
            float newY = Mathf.Lerp(position.y, endY, lerpSpeed * Time.deltaTime);
            position = new Vector3(positio*)n.x, newY, position.z);
            transform.position = position;*/
        }
    }
}