using UnityEngine;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Threading;
using System;
using System.Linq;
using Codice.Client.Common.GameUI;

/// <summary>
/// Handles the fishing behaviour
/// </summary>
[RequireComponent(typeof(FishingLineRenderer))]
public class Fishing : MonoBehaviour
{
    [SerializeField] FishingLineRenderer _fishingLineRenderer;
    [SerializeField] InputManager _inputManager; 
    [SerializeField] PlayerStateMachine _playerStateMachine; //reference to the player state machine, used to transition between states.
    [SerializeField] Transform _fishingRod;

    //rigidbody for the float, it will be thrown out from the rod, based on the cast distance
    [SerializeField] Rigidbody _floatRigidbody; 
    //the transform to parent the float to, so it's always launched from the same position (tip of the rod)
    [SerializeField] Transform _floatParentHomePosition; 
    //initial rotation of the fishing rod
    Quaternion _initialRodRot;

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
        _initialRodRot = _fishingRod.localRotation; // get the initial rotation of the fishing rod
        _fishingLineRenderer = GetComponent<FishingLineRenderer>();
    }
    
    void SubToInputs()
    {
        _inputManager.OnCastPressed += StartCast;
        _inputManager.OnRecallPressed += RecallLine; 
        _inputManager.OnCastReleased += EndCast;
        _inputManager.OnReelPressed += ReelLine; 

    }
    void OnDisable()
    {
        _inputManager.OnCastPressed -= StartCast;
        _inputManager.OnRecallPressed -= RecallLine;
        _inputManager.OnCastReleased -= EndCast;
        _inputManager.OnReelPressed -= ReelLine; 
    }

    #region Casting
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

        Quaternion initialRot = _fishingRod.localRotation; // get the initial rotation of the fishing rod
        Quaternion targetRot = Quaternion.Euler(initialRot.eulerAngles.x, initialRot.eulerAngles.y, initialRot.eulerAngles.z + 30f); // rotate the fishing rod 30 degrees on the z axis
        //lerp the fishing rod back over half a second
        float timer = 0.0f;
        float duration = 0.5f; // duration of the lerp
        // while(timer < duration)
        // {

        //     await Awaitable.NextFrameAsync(); //return to the main thread
        // }
        
        while (!_cts.Token.IsCancellationRequested)
        {
            timer += Time.deltaTime;
            _fishingRod.localRotation = Quaternion.Lerp(initialRot, targetRot, timer / duration);
            //increment the cast distacne over time, between the mix and max distance
            CastDistance = Mathf.Clamp(CastDistance + Time.deltaTime * 20f, _minCastDistance, _maxCastDistance);
            //return to the main thread
            await Awaitable.NextFrameAsync(); 
        }
        //TODO: do cast logic with the cast distance
        CastFloat(CastDistance);
    }
    /// <summary>
    /// Interupts the casting process, this will stop the casting and return to the fishing state
    /// </summary>
    async void EndCast()
    {

        // Stop casting
        _cts?.Cancel();
       
        //lerp the fishing rod back over half a second
         Quaternion initialRot = _fishingRod.localRotation; // get the initial rotation of the fishing rod
       //Quaternion targetRot = Quaternion.Euler(initialRot.eulerAngles.x, initialRot.eulerAngles.y, initialRot.eulerAngles.z - 30f); // rotate the fishing rod back to the original position
        float timer = 0.0f;
        float duration = 0.2f; // duration of the lerp
        while(timer < duration)
        {
            timer += Time.deltaTime;
            _fishingRod.localRotation = Quaternion.Lerp(initialRot, _initialRodRot, timer / duration);
            await Awaitable.NextFrameAsync(); //return to the main thread
        }
        //TODO: animate the fishing line going out
        _playerStateMachine.TransitionToState(PlayerState.Fishing);
    }

    private async void CastFloat(float castDistance)
    {
        if(_floatRigidbody == null) {Debug.LogError("No float rigidbody assigned"); return;}
        Debug.Log("Casting float with distance: " + castDistance);
        //reset the rb velocity
        _floatRigidbody.linearVelocity = Vector3.zero; 
        _floatRigidbody.angularVelocity = Vector3.zero; 
        _floatRigidbody.gameObject.SetActive(true); //activate the float rigidbody
        _floatRigidbody.transform.parent = null;
        _floatRigidbody.AddForce((transform.forward + transform.up*0.2f) * castDistance, ForceMode.Impulse); //add force to the float rigidbody in the direction of the players forward direction and up a little bit to simulate the float going out.

        Debug.Log("Render line to :" + _floatRigidbody.transform.gameObject.name);
        _fishingLineRenderer.AttachFishingLineToTransform(_floatRigidbody.transform);
    }
    #endregion
    #region Recall
    //recalling the line before catching a fish. Will bring the line back in and take us back to the moving state
    private void RecallLine()
    {
        //TODO: animate the line coming back in

        //recall the float
        _floatRigidbody.transform.parent = _floatParentHomePosition; 
        _floatRigidbody.transform.localPosition = Vector3.zero; //reset the position of the float to the home position
        _floatRigidbody.gameObject.SetActive(false); 
        //return to the moving state
        _playerStateMachine.TransitionToState(PlayerState.Moving);

    }

    #endregion
    #region Reeling
    //reeling in the line when a fish is on the hook. We will either break the line or catch the fish to move back to the moving state

    private void ReelLine()
    {
        //TODO: Animate the line reeling in

        //TODO: Increase the 'tension' meter on the line

        //bring the line in an incremental amount
    }
    #endregion
}