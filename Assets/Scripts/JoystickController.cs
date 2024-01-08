using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class JoystickController : MonoBehaviour
{
    [SerializeField]
    private bool useJoystick = false;

    // (0.1, -0.2) for the joystick
    [Tooltip("Added to input axes to correct neutral position.")]
    [SerializeField]
    private Vector2 neutralCorrection = new Vector2(0.1f, -0.2f);

    [SerializeField] private Vector2 deadzone = new Vector2(0.05f, 0.05f);

    [Tooltip("Range of motor rotation, in degrees, for each axis, in each direction.")]
    [SerializeField] private Vector2 motorAngleRange = new Vector2(30f, 30f);

    [SerializeField] private Vector2Int motorOffset = new Vector2Int(0, 0);

    [SerializeField] private float motorWriteInterval = 0.02f;

    [SerializeField] private Vector2 rotationSpeed = new Vector2(0.01f, 0.01f);

    [SerializeField] private float laserProbeRadius = 0.1f;

    [SerializeField] private SerialController serialController;

    [SerializeField] private AudioSource targetLockAudio;

    [Header("Debug")]
    [SerializeField] private Transform laser;
    [SerializeField] private TMP_Text horizontalDebugText;
    [SerializeField] private TMP_Text verticalDebugText;
    [SerializeField] private TMP_Text knobDebugText;

    [Range(0, 180)]
    [SerializeField] private int value;

    private Vector2 rot;
    private int laserState;
    private float frequency = 0;

    private bool alienLocked = false;

    private float motorWriteTimer = 0;

    public float Frequency => frequency;

    private void Start()
    {
        rot = Vector2.zero;
        laserState = 1;
        InitMotor();
    }

    private void Update()
    {
        UpdateRotation();
        InteractWithAliens();
        UpdateFrequency();
        motorWriteTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        laser.transform.position = new Vector3(rot.x, -rot.y, 0);
    }

    private void InitMotor()
    {
        if (serialController.enabled)
        {
            serialController.SendSerialMessage($"{90 + motorOffset.x} {90 + motorOffset.y} {laserState}");
        }
    }

    private void UpdateRotation()
    {
        Vector2 input = GetJoystickInput();
        DebugShowInput(input);

        int newLaserState = GetLaser();

        Vector2 direction = input.normalized;

        Vector2 newRot = new Vector2(
            Mathf.Clamp(rot.x + direction.x * rotationSpeed.x * Time.deltaTime, -1, 1),
            Mathf.Clamp(rot.y + direction.y * rotationSpeed.y * Time.deltaTime, -1, 1));

        Vector2 newRotMotor = WorldToMotor(newRot);
        Vector2 rotDiff = newRotMotor - WorldToMotor(rot);

        bool shouldMessageSerial = motorWriteTimer >= motorWriteInterval
            && (Mathf.Abs(rotDiff.x) > 0.999f || Mathf.Abs(rotDiff.y) > 0.999f
                || newLaserState != laserState);

        if (serialController.enabled && shouldMessageSerial)
        {
            serialController.SendSerialMessage($"{newRotMotor.x} {newRotMotor.y} {newLaserState}");

            motorWriteTimer = 0;
        }

        rot = newRot;
        laserState = newLaserState;
    }

    private void InteractWithAliens()
    {
        Collider2D collider = Physics2D.OverlapCircle(laser.transform.position, laserProbeRadius);
        Alien alien = collider?.GetComponent<Alien>();
        if (alien == null)
        {
            alienLocked = false;
            return;
        }

        if (!alienLocked)
        {
            targetLockAudio.Play();
            alienLocked = true;
        }

        bool shoot = Input.GetButtonDown("Fire1");
        bool accept = Input.GetButtonDown("Fire2");

        if (!shoot && !accept) return;

        if (shoot)
        {
            alien.Shoot();
            Debug.Log("Alien shot!");
        }
        else if (accept)
        {
            alien.Accept();
            Debug.Log("Granted access to alien!");
        }
    }

    private void UpdateFrequency()
    {
        if (useJoystick)
        {
            // [0.2, -0.95]
            float scroll = Input.GetAxis("Joystick Scroll");
            frequency = Mathf.InverseLerp(-0.95f, 0.2f, scroll);
        }
        else
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            frequency = Mathf.Clamp(frequency + scrollDelta, 0, 1f);
        }

        if (knobDebugText != null)
        {
            knobDebugText.text = $"Freq: {frequency}";
        }

        onFrequencyChanged?.Invoke(frequency);
    }

    private Vector2 GetJoystickInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Correct neutral position
        if (useJoystick)
        {
            h = h + neutralCorrection.x;
            v = v + neutralCorrection.y;
        }

        // Apply deadzone
        if (h > deadzone.x) h = 1;
        else if (h < -deadzone.x) h = -1;
        else h = 0;

        if (v > deadzone.y) v = 1;
        else if (v < -deadzone.y) v = -1;
        else v = 0;

        return new Vector2(h, v);
    }

    private int GetLaser()
    {
        return 1;
    }

    private void DebugShowInput(Vector2 input)
    {
        if (horizontalDebugText != null)
        {
            horizontalDebugText.text = $"X: {input.x.ToString()}";
        }

        if (verticalDebugText != null)
        {
            verticalDebugText.text = $"Y: {input.y.ToString()}";
        }
    }

    private Vector2 WorldToMotor(Vector2 world)
    {
        int motorHRot = (int)Mathf.Floor(-world.x * motorAngleRange.x);
        int motorVRot = (int)Mathf.Floor(world.y * motorAngleRange.y);

        motorHRot = motorHRot + 90 + motorOffset.x;
        motorVRot = motorVRot + 90 + motorOffset.y;

        return new Vector2(motorHRot, motorVRot);
    }

    private void OnApplicationQuit()
    {
        if (serialController.enabled)
        {
            Vector2 motorRot = WorldToMotor(rot);
            serialController.SendSerialMessage($"{motorRot.x} {motorRot.y} 0");
        }
    }

    public event Action<float> onFrequencyChanged;
}
