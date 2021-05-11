using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraOrbitalMotionController : MonoBehaviour
{
    private Vector2 _rotationXY;
    private Vector2 _initialXYRotation;
    private float _rotationSensitivityTime;
    private float _dampingTime;
    
    [Tooltip("Direct parent object of Camera")]
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Vector2 _horizonAngleClamp = new Vector2(175.0f, 185.0f);
    [SerializeField] private Vector2 _verticalAngleClamp = new Vector2(-5.0f, 15.0f);
    [SerializeField, Range(0, 50f)] private float _rotationSensitivity = 10f;
    [SerializeField, Range(0, 5)] private float _dampingSpeed = 1f;
    [SerializeField] private float _focalDistance = 10.456f;

    private void Start()
    {
        Input.gyro.enabled = true;

        _initialXYRotation.x = transform.rotation.eulerAngles.x;
        _initialXYRotation.y = transform.rotation.eulerAngles.y;
        
        _rotationXY.x = _initialXYRotation.x;
        _rotationXY.y = _initialXYRotation.y;
    }

    private void Update()
    {
        _rotationSensitivityTime = _rotationSensitivity * Time.deltaTime;
        _dampingTime = _dampingSpeed * Time.deltaTime;
        
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButton(1))
        {
            _rotationXY.y += Input.GetAxis("Mouse X") * 2f * _rotationSensitivityTime;
            _rotationXY.x -= Input.GetAxis("Mouse Y") * 2f * _rotationSensitivityTime;
        }
#elif UNITY_ANDROID || UNITY_IOS
        _rotationXY.x += Input.gyro.rotationRate.x * _rotationSensitivityTime;
        _rotationXY.y -= Input.gyro.rotationRate.y * _rotationSensitivityTime;
#endif
        _rotationXY.x = ClampAngle(_rotationXY.x, _verticalAngleClamp.x, _verticalAngleClamp.y);
        _rotationXY.y = ClampAngle(_rotationXY.y, _horizonAngleClamp.x, _horizonAngleClamp.y);

        var cameraRotation = Quaternion.Euler(_rotationXY.x, _rotationXY.y, 0.0f);
        var cameraPosition = cameraRotation * Vector3.forward * _focalDistance;

        transform.position = _cameraPivot.transform.position - cameraPosition;
        transform.LookAt(_cameraPivot);

        _rotationXY.x = Mathf.Lerp(_rotationXY.x, _initialXYRotation.x, _dampingTime);
        _rotationXY.y = Mathf.Lerp(_rotationXY.y, _initialXYRotation.y, _dampingTime);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }

        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}