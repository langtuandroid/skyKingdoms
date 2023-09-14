using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed = -10.0f;
    public float radius = 2.0f;
    public float height = 1.0f;  

    private void Update()
    {
        // Calcular la posición orbital
        float angle = orbitSpeed * Time.time;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, height, z) + target.position;
    }
}


public class SpecialAttack : MonoBehaviour
{
    [SerializeField] private List<GameObject> _fragmentsList;
    [SerializeField] private Transform _playerSpecialAttackPoint;
    private GameObject _fragment;

    public void Attack()
    {
        StartCoroutine(nameof(WaitUntilInstantiate));

    }

    private IEnumerator WaitUntilInstantiate(){
        yield return new WaitForSeconds(0.5f);
        
        _fragment = Instantiate(_fragmentsList[0], _playerSpecialAttackPoint);
        
        StartCoroutine(nameof(WaitUntilOrbit), _fragment);
    }



    private IEnumerator WaitUntilOrbit(GameObject fragment)
    {
        yield return new WaitForSeconds(0.5f);
        
        Orbit orbitScript = fragment.AddComponent<Orbit>();
        orbitScript.target = gameObject.transform;
        orbitScript.orbitSpeed = orbitScript.orbitSpeed;
        orbitScript.radius = orbitScript.radius;
    }
}