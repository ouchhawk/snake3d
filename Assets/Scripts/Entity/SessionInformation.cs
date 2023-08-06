using UnityEngine;

public class SessionInformation : MonoBehaviour
{
    public static long totalScore = 0;
    public static long snakeLength = 15;
    public static long level = 1;
    private static long initialTotalScore = 0;
    private static long initialSnakeLength = 15;
    private static long initialLevel = 1;


    public static void ResetScore()
    {
        totalScore = initialTotalScore;
    }
    public static void ResetSnakeLength()
    {
        snakeLength = initialSnakeLength;
    }
    public static void ResetLevel()
    {
        level = initialLevel;
    }
}
