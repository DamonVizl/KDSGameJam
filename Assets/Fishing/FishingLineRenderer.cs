using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLineRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [SerializeField] public Transform _rodTipTransform = null;
    [SerializeField] public Transform _target = null;
    
    public void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        
        _lineRenderer.startColor = Color.gray;
        _lineRenderer.endColor = Color.gray;
        
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.051f;
        
        _lineRenderer.positionCount = 2;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    public void FixedUpdate()
    {
        _lineRenderer.SetPosition(0, _rodTipTransform.position);
        _lineRenderer.SetPosition(1, _target.position);
    }

    public void AttachFishingLineToTransform(Transform target) => _target = target;
}