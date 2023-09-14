using System;
using System.Collections;
using Attacks;
using UnityEngine;
using Utils;

public class BlueFragment : MonoBehaviour
{
    [SerializeField] private GameObject _specialAttack;
    [SerializeField] private GameObject _superSpecialAttack;
    private GameObject _attack;
    private MagicAttack _magicAttack;
    private Rigidbody _rb;

    private bool _canCheckIfHit;

    private void Start()
    {
        StartCoroutine(nameof(WaitToChange));
    }

    private void Update()
    {
        if (!_canCheckIfHit) return;
        if (_magicAttack.CanHit)
        {
            StopObjectImmediately();
        }
    }

    private IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z + 1f);
        Transform playerTransform = GameObject.FindGameObjectWithTag(Constants.Player).GetComponent<Transform>();
        _attack = Instantiate(_specialAttack, position, playerTransform.rotation);
        _magicAttack = _attack.GetComponent<MagicAttack>();
        _canCheckIfHit = true;

        _rb = _attack.GetComponent<Rigidbody>();
        if (_rb != null)
        {
            // Obtener la dirección 'forward' del jugador en el espacio mundial
            Vector3 playerForward = playerTransform.TransformDirection(Vector3.forward);

            // Aplicar la fuerza en esa dirección
            _rb.AddForce(playerForward * 3f, ForceMode.Impulse);

            // Iniciar la corrutina para detener el objeto después de 5 segundos
            StartCoroutine(StopObjectAfterSeconds(3f));
        }

        //Destroy(_attack, 5f); // Destruir después de 10 segundos para dar tiempo a detenerse
        Destroy(gameObject, 5f);
    }

    private IEnumerator StopObjectAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (_rb != null) // Comprobar si el objeto aún existe
        {
            _rb.velocity = Vector3.zero;
        }
    }
    
    private void StopObjectImmediately()
    {
        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
        }
    }
}