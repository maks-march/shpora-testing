using System;
using System.Dynamic;

namespace TDD.Task;

public class Game
{
    private int Score = 0;
    private int[] frames = new int[10];
    private int currentFrame = 0;
    private (int bonus, int usageCount)[] bonuses = new (int, int)[10];
    private int throws = 0;

    public void Roll(int pins)
    {
        if (frames[currentFrame] + pins > 10 || pins < 0)
        {
            throw new Exception("SOSAL");
        }

        frames[currentFrame] += pins;
        throws++;
        
        UpdatePreviousFrame(pins);
        if (frames[currentFrame] == 10)
        {
            if (throws == 1)
            {
                bonuses[currentFrame] = (2, 2);
            }
            else
            {
                bonuses[currentFrame] = (1, 1);
            }
            currentFrame++;
            throws = 0;
        }
        else if (throws == 2)
        {
            throws = 0;
            currentFrame++;
            Score += pins;
        } else
        {
            Score += pins;
        }
    }
    
    private void UpdatePreviousFrame(int pins)
    {
        if (currentFrame > 0)
        {
            var prevBonus = bonuses[currentFrame - 1];
            if (prevBonus.bonus > 0 && prevBonus.usageCount > 0)
            {
                frames[currentFrame - 1] += pins;
                Score += pins + 10;
                --bonuses[currentFrame - 1].usageCount;
            }
        }
        if (currentFrame > 1)
        {
            var prevBonus = bonuses[currentFrame - 2];
            if (prevBonus.bonus > 1 && prevBonus.usageCount > 0)
            {
                frames[currentFrame - 2] += pins;
                Score += pins + 10;
                --bonuses[currentFrame - 2].usageCount;
            }
        } 
    }

    public int GetScore(int frame)
    {
        return frames[frame];
    }

    public int GetScore()
    {
        return Score;
    }
}


