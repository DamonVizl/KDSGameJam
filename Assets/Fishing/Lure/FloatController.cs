using UnityEngine;

public class FloatController : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private PlayerStateMachine _sm;
    Vector2 _inputVector;
    [SerializeField] private float _floatControlSpeed = 2f; // Speed of the float object
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputManager.OnFloatControlPressed += OnFloatControlPressed;
        _inputVector = Vector2.zero;
    }

    void OnFloatControlPressed(Vector2 input)
    {
        _inputVector = input;
        Debug.Log("Float control input: " + _inputVector);
    }
    void Update()
    {
        transform.rotation = Quaternion.identity;
        if(_inputVector != Vector2.zero && _sm.GetCurrentState() == PlayerState.Fishing)
        {
            // Apply the float control input to the float object
            // For example, you can move the float object based on the input vector
            transform.position += new Vector3(_inputVector.x, 0) * Time.deltaTime* _floatControlSpeed;
        }
        else if(_sm.GetCurrentState() == PlayerState.Fishing)
        {
            //apply damping force to stop the movement of the float object
            transform.position = Vector3.Lerp(transform.position, transform.position, Time.deltaTime* 100f);
        }
    }
}
