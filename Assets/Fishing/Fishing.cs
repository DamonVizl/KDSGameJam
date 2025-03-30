using UnityEngine;
using System.Threading;
using System;

/// <summary>
/// Handles the fishing behaviour
/// </summary>
[RequireComponent(typeof(FishingLineRenderer))]
public class Fishing : MonoBehaviour
{
    [SerializeField] InputManager _inputManager; 
    [SerializeField] PlayerStateMachine _playerStateMachine; //reference to the player state machine, used to transition between states.
    [SerializeField] Transform _fishingRod, _floatCastPoint; //where to cast the float from, with the rotation
    
    CameraSwitcher _cameraSwitcher;
    [SerializeField] FishingData _fishingData; //reference to the fishing data, used to get the cast distance and other data.

    [SerializeField] FishingLineRenderer _fishingLineRenderer;
    
    //rigidbody for the float, it will be thrown out from the rod, based on the cast distance
    [SerializeField] Rigidbody _floatRigidbody; 
    //the transform to parent the float to, so it's always launched from the same position (tip of the rod)
    [SerializeField] Transform _floatParentHomePosition; 
    //reference to the lure
    [SerializeField] Lure _lure; 

    //initial rotation of the fishing rod
    Quaternion _initialRodRot;

    //bool for if the player is 'reeling' in
    bool _isReeling = false;
    float _castDistance; // The distance the player can cast the fishing line
    public float CastDistance 
    {
        get { return _castDistance; }
        set { 
            _castDistance = value; 
            OnCastDistanceChanged?.Invoke(_castDistance, _fishingData.MinCastDistance, _fishingData.MaxCastDistance); 
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
        
        _cameraSwitcher = FindFirstObjectByType<CameraSwitcher>();
        _cameraSwitcher.SwitchToFirstPerson();
    }
    void FixedUpdate()
    {
        if(_isReeling) Reel();
    }
    void SubToInputs()
    {
        _inputManager.OnCastPressed += StartCast;
        _inputManager.OnRecallPressed += RecallLine; 
        _inputManager.OnCastReleased += EndCast;
        _inputManager.OnReelPressed += PressReelButton; 
        _inputManager.OnReelReleased += ReleaseReelButton;

    }
    void OnDisable()
    {
        _inputManager.OnCastPressed -= StartCast;
        _inputManager.OnRecallPressed -= RecallLine;
        _inputManager.OnCastReleased -= EndCast;
        _inputManager.OnReelPressed -= PressReelButton; 
        _inputManager.OnReelReleased -= ReleaseReelButton;
    }

    #region Casting
    /// <summary>
    /// Starts the casting process, this will increment the cast distance over time and then
    /// call some cast logic (NYE)
    /// </summary>
    async void StartCast()
    {
        _cameraSwitcher.SwitchToCasting();

        if(_playerStateMachine.GetCurrentState() != PlayerState.Moving) return; //shouldn't be required because of the action maps but added regardless.
        CastDistance = 10f;
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        Quaternion initialRot = _fishingRod.localRotation; // get the initial rotation of the fishing rod
        Quaternion targetRot = Quaternion.Euler(initialRot.eulerAngles.x - 45f, initialRot.eulerAngles.y, initialRot.eulerAngles.z ); // rotate the fishing rod 30 degrees on the z axis
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
            CastDistance = Mathf.Clamp(CastDistance + Time.deltaTime * _fishingData.CastMultiplier, _fishingData.MinCastDistance, _fishingData.MaxCastDistance);
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
        
        _cameraSwitcher.SwitchToFirstPerson();
    }

    private void CastFloat(float castDistance)
    {
        if(_floatRigidbody == null) {Debug.LogError("No float rigidbody assigned"); return;}
        Debug.Log("Casting float with distance: " + castDistance);
        //reset the rb velocity
        _floatRigidbody.linearVelocity = Vector3.zero; 
        _floatRigidbody.angularVelocity = Vector3.zero; 
        _floatRigidbody.gameObject.SetActive(true); //activate the float rigidbody
        _floatRigidbody.transform.parent = null;
        Debug.DrawRay(_floatCastPoint.position, _floatCastPoint.transform.forward * castDistance, Color.red, 2f); //draw a ray in the direction of the cast point for debugging
        _floatRigidbody.AddForce((_floatCastPoint.transform.forward + Vector3.up*castDistance*0.01f) * castDistance, ForceMode.Impulse); //add force to the float rigidbody in the direction of the players forward direction and up a little bit to simulate the float going out.
    }
    #endregion
    #region Recall
    //recalling the line before catching a fish. Will bring the line back in and take us back to the moving state
    private void RecallLine()
    {
        //Clear the list of IAmMagnetic objects on the lure.
        _lure.Score();
        _lure.Reset();

        //score the fish in the collection
        //_scorer.AddFishToCollectionIfBigger()); //add the fish to the collection if its bigger than the current fish in the collection

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
    void Reel(){
        //get the vector of the float to the player
        Vector3 direction = this.transform.position - _floatRigidbody.transform.position; 
        direction.Normalize(); 
        //add force to the float in the direction of the player
        _floatRigidbody.AddForce(direction * _fishingData.ReelStrengthMultiplier, ForceMode.Impulse); 

        //check if the float is close enough to the player to catch the fish
        if(Vector3.Distance(this.transform.position, _floatRigidbody.transform.position) < 5.0f)
        {
            //catch the fish and return to the moving state
            Debug.Log("Line has been fully reeled in");
            RecallLine();
        }
    }

    private void PressReelButton()
    {
        _isReeling = true; 
    }
    private void ReleaseReelButton()
    {
        _isReeling = false;
    }
    #endregion
}
