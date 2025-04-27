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
    private AudioSource _audioSource;
    private Player _player;

    private void Start()
    {
        // Set the initial value of the progress bar
        current = minimum;
        mask.fillAmount = 0;
        
        // Get the AudioSource component
        _audioSource = GetComponent<AudioSource>();
        
        // Get the script of the Player object
        _player = GameObject.Find("Player").GetComponent<Player>();
        
    }
    
    private void Update()
    {
        // Increase progress bar
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            current += increaseValue;
        }
        
        // Reset progress bar
        if (Input.GetKeyDown(KeyCode.R))
        {
            current = 0;
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

        if (UpdateFillAmount() == 1)
        {
            current = 0;
            _audioSource.Play();
            _player.SetIsMoving(false);
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
} 
