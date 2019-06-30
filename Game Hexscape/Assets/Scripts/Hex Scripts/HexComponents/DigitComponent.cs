using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DigitComponent : MonoBehaviour
{

    List<Hex> digitHexes;

    public int leadingZeroCount;
    public int numberToDisplay;

    DigitComponent()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDisplayValue(int value)
    {
        int[] digitsToDisplay = SplitDigits(value);
        while (digitsToDisplay.Length > digitHexes.Count)
        {
            if (!SpawnNewDigit()) break;
        }

        ////////// LOOP
        

       //this.gameObject.GetComponent<Hex>().hex
    }

    void InitialiseLeadingHexes()
    {
        for (int i = 0; i < Mathf.Abs(leadingZeroCount); i++)
        {
            if (!SpawnNewDigit()) break;
        }
    }

    //Spawns a new leading digit
    bool SpawnNewDigit()
    {
        Vector2Int lastHexIndex = MapSpawner.Instance.grid.WorldToCell(digitHexes.Last().gameObject.transform.position);
        Hex spawnedHex = MapSpawner.Instance.SpawnHexAtLocation(lastHexIndex - new Vector2Int( (leadingZeroCount > 0 ? 1 : -1) , 0), HexTypeEnum.HexTile_Digit0, false);
        if (spawnedHex)
        {
            digitHexes.Add(spawnedHex);
            return true;
        }
        else return false;
    }

    int[] SplitDigits(int num)
    {
        List<int> listOfInts = new List<int>();
        while (num > 0)
        {
            listOfInts.Add(num % 10);
            num = num / 10;
        }
        listOfInts.Reverse();
        return listOfInts.ToArray();
    }
}
