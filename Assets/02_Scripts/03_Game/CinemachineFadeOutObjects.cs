using Cinemachine;
using UnityEngine;

public class CinemachineFadeOutObjects : CinemachineExtension
{
    [SerializeField] private Material _fadeOutMaterial;
    [SerializeField] private float _lookAtTargetRadius;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _minDistance;
    [SerializeField] private bool _setToCameraToLookAtDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        
    }
}
