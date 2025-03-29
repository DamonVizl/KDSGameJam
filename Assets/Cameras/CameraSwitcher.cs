using System;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    [SerializeField] Camera _fpsCamera;
    [SerializeField] Camera _castingCamera;
    [SerializeField] MeshRenderer _playerModelRenderer;
    
    private PlayerStateMachine _playerStateMachine;
    private PlayerState _lastState;

    private void Start() => 
        _playerStateMachine = FindFirstObjectByType<PlayerStateMachine>();

    private void Update()
    {
        var currentState = _playerStateMachine.GetCurrentState();
        if (currentState != _lastState) 
            ProcessUpdatedState(currentState);

        _lastState = currentState;
    }

    private void ProcessUpdatedState(PlayerState currentState)
    {
        switch (currentState)
        {
            case PlayerState.Moving:
                SwitchToFirstPerson();
                break;
            case PlayerState.Fishing:
                SwitchToCasting();
                break;
        }
    }

    public void SwitchToCasting() => SetCasting(true);
    public void SwitchToFirstPerson() => SetCasting(false);

    private void SetCasting(bool isCasting)
    {
        _castingCamera.gameObject.SetActive(isCasting);
        _fpsCamera.gameObject.SetActive(!isCasting);
        _mainCamera.gameObject.SetActive(!isCasting);
        _playerModelRenderer.enabled = isCasting;
    }
}