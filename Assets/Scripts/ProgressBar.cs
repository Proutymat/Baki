using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{

    [SerializeField] private Image mask;
    [SerializeField] private float minimum = 0;
    [SerializeField] private float maximum;
    [SerializeField] private float current;
    
    [SerializeField] private Slider increaseSlider;
    [SerializeField] private Slider decreaseSlider;

    
    
    private float _timer;
    void Update()
    {
        // Increase progress bar
        if (Input.GetMouseButtonDown(0))
        {
            current += increaseSlider.value;
        }

        // Decrease progress bar every millisecond
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _timer = 0.001F;
            current -= decreaseSlider.value / 100;
        }
        
        current = current < minimum ? minimum : current > maximum ? maximum : current; // Clamp 'current' values to min and max
        
        UpdateFillAmount();
    }

    float UpdateFillAmount()
    {
        float currentOffset = current -  minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = (float)current / maximumOffset;
        mask.fillAmount = fillAmount;
        return fillAmount;
    }
} 
