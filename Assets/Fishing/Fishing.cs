using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Threading;
using System;

/// <summary>
/// Handles the fishing behaviour
/// </summary>
public class Fishing : MonoBehaviour
{
    [SerializeField] InputManager _inputManager; 
    [SerializeField] PlayerStateMachine _playerStateMachine; //reference to the player state machine, used to transition between states.

    float _minCastDistance = 10f;
    float _maxCastDistance = 100f;
    float _castDistance; // The distance the player can cast the fishing line
    public float CastDistance 
    {
        get { return _castDistance; }
        set { 
            _castDistance = value; 
            OnCastDistanceChanged?.Invoke(_castDistance, _minCastDistance, _maxCastDistance); 
            }
    }

    CancellationTokenSource _cts; 

    #region Events
    public static Action<float, float, float> OnCastDistanceChanged; //event to notify when cast distance changes, can be used for UI updates. 
    #endregion
    void Start()
    {
      SubToInputs();
    }
    void SubToInputs()
    {
        _inputManager.OnCastPressed += StartCast;
        _inputManager.OnCastReleased += EndCast;
    }
    void OnDisable()
    {
        _inputManager.OnCastPressed -= StartCast;
        _inputManager.OnCastReleased -= EndCast;
    }

    /// <summary>
    /// Starts the casting process, this will increment the cast distance over time and then
    /// call some cast logic (NYE)
    /// </summary>
    async void StartCast()
    {
        if(_playerStateMachine.GetCurrentState() != PlayerState.Moving) return; //shouldn't be required because of the action maps but added regardless.
        CastDistance = 10f;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        while (!_cts.Token.IsCancellationRequested)
        {
            //increment the cast distacne over time, between the mix and max distance
            CastDistance = Mathf.Clamp(CastDistance + Time.deltaTime * 100f, _minCastDistance, _maxCastDistance);
            //return to the main thread
            await Awaitable.NextFrameAsync(); 
        }
        //TODO: do cast logic with the cast distance
    }
    /// <summary>
    /// Interupts the casting process, this will stop the casting and return to the fishing state
    /// </summary>
    void EndCast()
    {
        // Stop casting
        Debug.Log("Stopped casting");
        _cts?.Cancel();
        _playerStateMachine.TransitionToState(PlayerState.Fishing);
    }
}