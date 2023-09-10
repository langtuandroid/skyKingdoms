using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;

public class LivingParticleController : MonoBehaviour {

    public Transform affector;

    private ParticleSystemRenderer psr;

    private bool _canStart;
    
    private void OnDisable()
    {
	    PlayerData.PlayerInstantiated -= HandlePlayerInstantiated;
    }
    
	void Start () {
        psr = GetComponent<ParticleSystemRenderer>();
        PlayerData.PlayerInstantiated += HandlePlayerInstantiated;
	}
	
	void Update () {
		if(_canStart)
			psr.material.SetVector("_Affector", affector.position);
    }
	
	private void HandlePlayerInstantiated(Transform playerTransform)
	{
		affector = playerTransform.Find("root");
		_canStart = true;
	}
}
