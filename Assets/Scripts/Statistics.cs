using UnityEngine;

public class Statistics
{
    private int nbUnitTraveled;
    private int nbLandmarksReached;
    private int nbWallsHit;
    private int nbDirectionChanges;
    private int nbButtonsPressed;
    private float timeSpentMoving;
    private int nbQuestionsAnswered;
    private int nbLeftAnswers;
    private int nbRightAnswers;
    private float timeBetweenQuestions;
    private float shortestTimeBetweenQuestions;
    private float longestTimeBetweenQuestions;
    private int nbProgressBarFull;
    
    // Getters and setters for each variable
    public int NbUnitTraveled { get => nbUnitTraveled; set => nbUnitTraveled = value; }
    public int NbLandmarksReached { get => nbLandmarksReached; set => nbLandmarksReached = value; }
    public int NbWallsHit { get => nbWallsHit; set => nbWallsHit = value; }
    public int NbDirectionChanges { get => nbDirectionChanges; set => nbDirectionChanges = value; }
    public int NbButtonsPressed { get => nbButtonsPressed; set => nbButtonsPressed = value; }
    public float TimeSpentMoving { get => timeSpentMoving; set => timeSpentMoving = value; }
    public int NbQuestionsAnswered { get => nbQuestionsAnswered; set => nbQuestionsAnswered = value; }
    public int NbLeftAnswers { get => nbLeftAnswers; set => nbLeftAnswers = value; }
    public int NbRightAnswers { get => nbRightAnswers; set => nbRightAnswers = value; }
    public float TimeBetweenQuestions { get => timeBetweenQuestions; set => timeBetweenQuestions = value; }
    public float ShortestTimeBetweenQuestions { get => shortestTimeBetweenQuestions; set => shortestTimeBetweenQuestions = value; }
    public float LongestTimeBetweenQuestions { get => longestTimeBetweenQuestions; set => longestTimeBetweenQuestions = value; }
    public int NbProgressBarFull { get => nbProgressBarFull; set => nbProgressBarFull = value; }
    
    public Statistics()
    {
        Reset();
    }

    public void Reset()
    {
        nbUnitTraveled = 0;
        nbLandmarksReached = 0;
        nbWallsHit = 0;
        nbDirectionChanges = 0;
        nbButtonsPressed = 0;
        timeSpentMoving = 0f;
        nbQuestionsAnswered = 0;
        nbLeftAnswers = 0;
        nbRightAnswers = 0;
        timeBetweenQuestions = 0f;
        shortestTimeBetweenQuestions = float.MaxValue;
        longestTimeBetweenQuestions = float.MinValue;
        nbProgressBarFull = 0;
    }
}
