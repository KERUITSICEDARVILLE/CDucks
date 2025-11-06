using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "duckStats", menuName = "Scriptable Objects/duckStats")]
public class duckStats : ScriptableObject
{
    /**************************************************************************
    * 
    * Name: Duck Stats
    * Creator: Kale
    *
    * Description: This file will handle storage of the stats/levels of the duck
    *
    ***************************************************************************/

    //Base Stats
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private float baseRainbowChance = 1 / 200; //This is 1/200 per-second
    
    //Level Array for increasing base values and reloading from save
    [SerializeField] private int[] duckLevels = { 1, 1, 1 };

    //Basic Accessors
    public int getHealth() { return baseHealth + (duckLevels[0] * 10); }
    public int getDamage() { return baseDamage + (duckLevels[1]); }
    public float getRainbowChance() { return baseRainbowChance + (duckLevels[2] * 0.005f); }

    // levelUp takes either "health", "damage", or "rainbow" and levels that field up by 1
    public void levelUp(string type)
    {
        if (type == "health") duckLevels[0] += 1;
        else if (type == "damage") duckLevels[1] += 1;
        else if (type == "rainbow") duckLevels[2] += 1;
    }
}

