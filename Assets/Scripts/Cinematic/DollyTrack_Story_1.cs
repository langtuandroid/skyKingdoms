using System.Collections;
using Cinemachine;
using Managers;
using Service;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class DollyTrack_Story_1 : InputsUI
{
    #region VARIABLES
    public CinemachineDollyCart dollyCartSeq1;
    public CinemachineDollyCart dollyCartSeq2;
    public CinemachineVirtualCamera finalCam;

    public GameObject player;
    public CinemachineFreeLook playerCam;
    private Animator _anim;

    private MyDialogueManager _myDialogueManager;
    
    #endregion
    
    private void Start()
    {
        ServiceLocator.GetService<MyLevelManager>().StartLevel();
        _myDialogueManager = ServiceLocator.GetService<MyDialogueManager>();
        player = GameObject.FindWithTag(Constants.Player).gameObject;
        player.transform.SetParent(dollyCartSeq2.transform);
        _anim = player.GetComponent<Animator>();
        
        if (ServiceLocator.GetService<MyLevelManager>().backToScene)
        {
            GameControl();
            SetDragonPosition();
        }
        else
        {
            if (dollyCartSeq1.m_Path != null)
                dollyCartSeq1.m_Speed = 12;
        
            if (dollyCartSeq2.m_Path != null)
                dollyCartSeq2.m_Speed = 0;
        
    
            _anim.SetBool("walk", false);
        
            StartCoroutine(DollyCart());
        }
    }
    
    private IEnumerator DollyCart()
    {
        Debug.Log("Ya he entrado en el dolly cart");
        _myDialogueManager.Init(); 
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);

        //**************************************Cambio de camara
        finalCam.enabled = true;
        //******* Player Camina acercandose al Dragon
        dollyCartSeq2.m_Speed = 2;
        _anim.SetBool("walk", true);
        yield return new WaitForSeconds(2.5f);
        dollyCartSeq2.m_Speed = 0;
        _anim.SetBool("walk", false);
        //********* Player se para
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);
        
        _myDialogueManager.NextText();
        yield return new WaitUntil(() => _myDialogueManager.CanContinue());
        yield return new WaitForSeconds(1);

        GameControl();
    }

    private void GameControl()
    {
        StopAllCoroutines();
        //Dragon.transform.DOMove(DragonIdlePosition.position, 1f).SetEase(Ease.Linear).Play();
        _myDialogueManager.StopStory();
        playerCam.LookAt = player.GetComponent<Transform>();
        playerCam.Follow = player.GetComponent<Transform>();
        playerCam.enabled = true;
        finalCam.enabled = false;
        player.GetComponent<PlayerController>().CanMove = true;
    }

    private void SetDragonPosition()
    {
        dollyCartSeq1.m_Position = dollyCartSeq1.m_Path.PathLength;
    }
    
    protected override void OnSubmit(InputAction.CallbackContext context)
    {
        base.OnSubmit(context);
        
          GameControl();
          SetDragonPosition();
    }

}
