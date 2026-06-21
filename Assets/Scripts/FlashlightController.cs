using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    private float maxBattery = 100f;
    private float drainRate = 1f;

    private float currentBattery;
    private bool isOn = true;
    private float baseIntensity = 2.5f;
    private Coroutine flickerCoroutine;
    private bool isPlayerInRange = false;

    void Start()
    {
        currentBattery = maxBattery;
    }


    public void OnFlashlight(InputValue value)
    {
        if (value.isPressed)
        {
            toggleFlashlight();
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            //if (flickerCoroutine == null)
            //{
            //    flickerCoroutine = StartCoroutine(flicker());
            //}
            if (isPlayerInRange)
            {
                BatteryUp();
            }

        }
    }


    public void toggleFlashlight()
    {
        if (!isOn && currentBattery >0f)
        {
            turnOnFlashlight();
        }
        else if(isOn) 
        {
            turnOffFlashlight();
        }
    }


private void turnOffFlashlight()
    {
        isOn = false;
        flashlight.intensity = 0.1f;
        flashlight.range = 5f;

        //stop stop
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

    }

    private void turnOnFlashlight()
    {
        flashlight.enabled = true;
        isOn = true;
        flashlight.range = 10f;
        flashlight.intensity = baseIntensity;
    }

    private void BatteryUp()
    {
        currentBattery += 20f;
        
    }

    private IEnumerator flicker()
    {
        float flickerDuration = Random.Range(1f, 5f);
        float elapsedTime = 0f;

        while (elapsedTime < flickerDuration && isOn)//
        {
            flashlight.intensity = Random.Range(0.1f, 1f);
            float randomDelay = Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(randomDelay);
            elapsedTime += randomDelay;
        }
        if(isOn)
        {
            flashlight.intensity = baseIntensity;
        }
        else
        {
            flashlight.intensity = 0.1f;
        }
        flickerCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Battery"))
        {
            isPlayerInRange = false;
        }
    }

    void Update()
    {
        if (isOn)
        {
            currentBattery -= drainRate * Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

            if (currentBattery <= 0f)
            {
                turnOffFlashlight();
            }

            if (currentBattery < maxBattery * 0.2f)
            {
                flickerCoroutine = StartCoroutine(flicker());
            }
        }
    }
}