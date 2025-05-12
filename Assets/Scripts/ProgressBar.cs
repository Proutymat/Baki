using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    [SerializeField] private Image mask;
    [SerializeField] private float minimum = 0;
    [SerializeField] private float maximum;
    [SerializeField] private float current;

    [SerializeField] private float increaseValue;
    [SerializeField] private float decreaseValue;
    
    private float _timer;
    private bool barIsEmpty;

    private void Start()
    {
        // Set the initial value of the progress bar
        current = minimum;
        mask.fillAmount = 0;

        barIsEmpty = true;
    }
    
    private void Update()
    {
        // DEBUG : Increase progress bar
        if (Input.GetKeyDown(KeyCode.B) && Input.GetKeyDown(KeyCode.A) && Input.GetKeyDown(KeyCode.R))
        {
            current += increaseValue;
        }
        
        // DEBUG : Reset progress bar
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.S))
        {
            current = 0;
            barIsEmpty = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionBarEmpty");
        }
    }

    void FixedUpdate()
    {
        // Decrease progress bar every millisecond
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _timer = 0.001F;
            current -= decreaseValue / 100;
        }

        current = current < minimum ? minimum : current > maximum ? maximum : current; // Clamp 'current' values to min and max
        
        if (current < 0 && !barIsEmpty)
        {
            barIsEmpty = true;
            current = 0;
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionBarEmpty");
        }
        
        if (UpdateFillAmount() == 1)
        {
            current = 0;
            barIsEmpty = true;
        }

    }

    float UpdateFillAmount()
    {
        float currentOffset = current -  minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = (float)current / maximumOffset;
        mask.fillAmount = fillAmount;
        return fillAmount;
    }
    
    public bool IncreaseProgressBar()
    {
        current += increaseValue;
        
        if (current >= maximum)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/UI/UI_InGame/UI_IG_QuestionBarFull");
            return true;
        }
        else
        {
            barIsEmpty = false;
            return false;
        }
    }
}
