using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class platform : MonoBehaviour
{
    enum DirectionState: int
    {
        None = 0,
        Forward = 1,
        Backward = -1
    }

    enum HeightState : int
    {
        None = 0,
        Up = 1,
        Down = -1
    }

    private bool _rotating_controller_handled = false;
    public Unity.VRTemplate.XRKnob rotating_controller_knob;
    public float rotating_controller_handle_angular_speed = 5.0f;

    public float forward_speed = 5.0f;
    public float up_speed = 5.0f;
    public float angular_speed = 30.0f;

    public Unity.VRTemplate.XRKnob height_controller_knob;
    public Unity.VRTemplate.XRKnob direction_controller_knob;

    private HeightState _height_state = HeightState.None;
    private DirectionState _direction_state = DirectionState.None;

    public MeshRenderer height_up_detector;
    public MeshRenderer height_down_detector;
    public MeshRenderer height_none_detector;

    public MeshRenderer direction_up_detector;
    public MeshRenderer direction_down_detector;
    public MeshRenderer direction_none_detector;

    public Material enabled_detector_material;
    public Material disabled_detector_material;

    public Transform center;

    public float max_height = 150.0f;

    public Transform player;

    void RefreshDetectors()
    {
        height_up_detector.material = disabled_detector_material;
        height_down_detector.material = disabled_detector_material;
        height_none_detector.material = disabled_detector_material;

        if (height_controller_knob.value > 0.75f)
        {
            height_up_detector.material = enabled_detector_material;
            _height_state = HeightState.Up;
        }
        else if (height_controller_knob.value < 0.25f)
        {
            height_down_detector.material = enabled_detector_material;
            _height_state = HeightState.Down;
        }
        else
        {
            height_none_detector.material = enabled_detector_material;
            _height_state = HeightState.None;
        }

        direction_up_detector.material = disabled_detector_material;
        direction_down_detector.material = disabled_detector_material;
        direction_none_detector.material = disabled_detector_material;

        if (direction_controller_knob.value > 0.75f)
        {
            direction_up_detector.material = enabled_detector_material;
            _direction_state = DirectionState.Forward;
        }
        else if(direction_controller_knob.value < 0.25f)
        {
            direction_down_detector.material = enabled_detector_material;
            _direction_state = DirectionState.Backward;
        }
        else
        {
            direction_none_detector.material = enabled_detector_material;
            _direction_state = DirectionState.None;
        }
    }

    void Start()
    {
        RefreshDetectors();
    }

    private bool IsGround()
    {
        RaycastHit hit;
        /*if (Physics.Raycast(center.position, -Vector3.up, out hit, 1))
        {
            return true;
        }*/

        if (Physics.BoxCast(center.position + new Vector3(0, 0.1f, 0), new Vector3(2, 0.05f, 2), -Vector3.up, out hit, transform.rotation, 0.05f))
        {
            if (hit.collider.tag == "Landscape") {
                return true;
            }
        }

        return false;
    }

    void Update()
    {
        // сбрасывем поворот руля, если отпустили
        if (!_rotating_controller_handled)
        {
            rotating_controller_knob.value = Mathf.Lerp(rotating_controller_knob.value, 0.5f, Time.deltaTime * rotating_controller_handle_angular_speed);
        }

        if (_height_state == HeightState.Up)
        {
            transform.position += Vector3.up * up_speed * Time.deltaTime;
            if (transform.position.y > max_height)
            {
                transform.position = new Vector3(transform.position.x, max_height, transform.position.z);
            }
        }
        else if (_height_state == HeightState.Down)
        {
            if (!IsGround())
            {
                transform.position += Vector3.up * -up_speed * Time.deltaTime;
            }
        }

        if (!IsGround())
        {

            // совершаем поворот, если руль имеет абсолютное значение поворта >  0.1f
            if (Mathf.Abs(rotating_controller_knob.value - 0.5f) > 0.1f)
            {
                transform.eulerAngles += Vector3.up * (rotating_controller_knob.value - 0.5f) * angular_speed * Time.deltaTime;
            }

            if (_direction_state == DirectionState.Forward)
            {
                transform.position += transform.forward * forward_speed * Time.deltaTime;
            }
            else if (_direction_state == DirectionState.Backward)
            {
                transform.position += transform.forward * -forward_speed * Time.deltaTime;
            }
        }

        //player.transform.position = transform.position;
    }

    public void OnRotatingControllerEnter()
    {
        Debug.Log("OnRotatingControllerEnter");
        _rotating_controller_handled = true;
    }

    public void OnRotatingControllerLeave()
    {
        Debug.Log("OnRotatingControllerLeave");
        _rotating_controller_handled = false;
    }

    public void OnHeightControllerValueChanged()
    {
        RefreshDetectors();
    }

    public void OnDirectionControllerValueChanged()
    {
        RefreshDetectors();
    }
}
