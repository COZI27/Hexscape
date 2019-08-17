using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class DigitComponent : MonoBehaviour
{


    List<Hex> digitHexes;
    List<int> digitValues;


    public int leadingZeroCount = 6;

    // the initial numberToDisplay
    public int numberToDisplay;

    DigitComponent() 
    {

    }


    // Note: Digit array could be stored elsewhere (HexBank?)
    HexTypeEnum[] hexDigits = { HexTypeEnum.HexTile_Digit0,
                                HexTypeEnum.HexTile_Digit1,
                                HexTypeEnum.HexTile_Digit2,
                                HexTypeEnum.HexTile_Digit3,
                                HexTypeEnum.HexTile_Digit4,
                                HexTypeEnum.HexTile_Digit5,
                                HexTypeEnum.HexTile_Digit6,
                                HexTypeEnum.HexTile_Digit7,
                                HexTypeEnum.HexTile_Digit8,
                                HexTypeEnum.HexTile_Digit9,
    };


    float timer = 10;

    
    private void OnDestroy()
    {
        // Reset the hex to default for future spawning. Note: It may be possible to simply remove this object from play/ destroy it if required.
        this.GetComponent<Hex>().enabled = true;
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <=0)
        {
            int randVal = Random.Range(0, 99999);
            UpdateDisplayValue(randVal);
            Debug.Log("randVal = " + randVal);
            timer = 10;
        }
    }


 

    // Start is called before the first frame update
    void Start()
    {
        // Desable the renderer and hex functionality of this components hex tile
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<Hex>().enabled = false; // TEMP - TODO: Figure out a better wy to prevent hex from being enabled/ visible

        digitHexes = new List<Hex>();
        digitValues = new List<int>();

        InitialiseLeadingHexes();

        Hex owniingHex = this.gameObject.GetComponent<Hex>();
        owniingHex.onHexDig += DestroyDigitComponent;


        //UpdateDisplayValue(0);
    }

    public void DestroyDigitComponent()
    {
        Debug.Log("DestroyDigitComponent");
        Destroy(this);
    }

    public void UpdateDisplayValue(int valueToDisplay)
    {
        Debug.Log("UpdateDisplayValue = " + valueToDisplay);


        // Replace current display value with new value
        int[] digitsToDisplay = ParseScore(valueToDisplay);
        digitValues.Clear();
        foreach (int i in digitsToDisplay)
            digitValues.Add(i);

        // Add the leading zeros if applicable
        if (digitValues.Count < leadingZeroCount)
        {
            int leadingZeroCounter = leadingZeroCount - digitValues.Count;
            for (int i = 0; i < leadingZeroCounter; i++)
            {
                digitValues.Add(0);
            }
        }


   
        int tempCOunt = 0;

        // Add hexes to accomodate for integer length
        while (digitValues.Count > digitHexes.Count)
        {
            Debug.Log("digitValues.Count > digitHexes.Count. " + ++tempCOunt);
            Debug.Log(digitValues.Count + " > " + digitHexes.Count);
            if (!SpawnNewDigit()) break;
        }

 
        for (int i = 0; i < digitHexes.Count - 1; i++)
        {
            ReplaceDigit(i, hexDigits[digitValues[i]]);

            if (i > digitValues.Count)
            {
                digitHexes[i].DigHex();
            }

            //if (i < digitValues.Count)
            //    ReplaceDigit(i, hexDigits[digitValues[i]]);
            //else if (leadingZeroCounter > 0)
            //{
            //    ReplaceDigit(i, HexTypeEnum.HexTile_Digit0);
            //    --leadingZeroCounter;
            //}
            //else // Remove the hex
            //{
            //    digitHexes[i].DigHex();
            //}
        }




        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void UpdateDisplayValue_DEPRECATED(int scoreToDisplay)
    {
        Debug.Log("UpdateDisplayValue DEPRECATED");

        int[] digitsToDisplay = ParseScore(scoreToDisplay);
        int index = 0; // index is used to itterate through the array of hex objects

        Queue<GameObject> digitObjectQueue = new Queue<GameObject>();

       // Debug.Log("digit hexes length  = " + digitHexes.Length);

        foreach (Hex hex in digitHexes)
        {

            if (hex != null)
            {
                hex.SetFlag(true);
                digitObjectQueue.Enqueue(hex.gameObject);
            }
        }

        digitObjectQueue.Reverse(); // Didn't work?

        while (digitObjectQueue.Count > 0)
        {

            HexTypeEnum typeToDisplay;

            if (index > digitsToDisplay.Length - 1) {
                // Set next hex to 'zero'
                typeToDisplay = HexTypeEnum.HexTile_Digit0;
                Debug.Log(0);
            }
            else {
                // Set next hex to value in array index 
                typeToDisplay = hexDigits[digitsToDisplay[index]];
            }


            if (index > digitObjectQueue.Count - 1 && index < digitsToDisplay.Length - 1 )
            {
                if (SpawnNewDigit())
                {
                    digitObjectQueue.Enqueue(digitHexes[digitHexes.Count - 1].gameObject);
                }
                else break; // FAILED TO SPAWN
            }

            GameObject digitToModify = digitObjectQueue.Dequeue();

            //Debug.Log(digitsToDisplay.Length - 1 + " | " + index + " | " + digitsToDisplay[index]);

            //MapSpawner.Instance.SpawnHexAtLocation(MapSpawner.Instance.grid.WorldToCell(digitToModify.transform.position), typeToDisplay, true);

            ReplaceDigit(index, typeToDisplay);

            index++;
        } 
    }

    private int[] ParseScore(int score)
    {
        int valueToParse = score;

        List<int> digits = new List<int>();
        do
        {
            digits.Add(valueToParse % 10);
            Debug.Log(valueToParse + " remainder = " + valueToParse % 10);
            valueToParse /= 10;

        } while (valueToParse > 0);

        return digits.ToArray();
    }

    private void InitialiseLeadingHexes()
    {
        for (int i = 0; i < leadingZeroCount + 1; i++)
            if (!SpawnNewDigit()) break;
    }

    private bool ReplaceDigit(int index, HexTypeEnum typeToDisplay)
    {
        Hex newHex = MapSpawner.Instance.SpawnHexAtLocation(MapSpawner.Instance.grid.WorldToCell(digitHexes[index].transform.position), typeToDisplay, true, MapSpawner.MAP_LAYER_DIGIT);

        //TODO: Move Component to new hex
        DigitComponent component = this;
        digitHexes[index].DigHex();
        digitHexes[index] = newHex;
        return true;
    }

    //Spawns a new leading digit
    private bool SpawnNewDigit(HexTypeEnum digitToAdd = HexTypeEnum.HexTile_Digit0)
    {
        Vector2Int nextHexCoord = MapSpawner.Instance.grid.WorldToCell(this.gameObject.transform.position) - new Vector2Int(digitHexes.Count, 0);

        Hex spawnedHex = MapSpawner.Instance.SpawnHexAtLocation(
            nextHexCoord,
            digitToAdd,
            true,
            MapSpawner.MAP_LAYER_DIGIT
            );

        if (spawnedHex)
        {
            digitHexes.Add(spawnedHex);
            //digitValues.Add(0);


            //Hex[] extendedDigitArray = new Hex[digitHexes.Count + 1];

            //for (int i = 0; i < digitHexes.Length - 1; ++i)
            //{
            //    extendedDigitArray[i] = digitHexes[i];
            //}

            //extendedDigitArray[digitHexes.Length - 1] = spawnedHex;

            //Debug.Log("spawnedHex.gameObject = " + spawnedHex.gameObject);
            //Debug.Log("digit hexes length pre extend = " + digitHexes.Length);

            //digitHexes = extendedDigitArray;

            //Debug.Log("digit hexes length post extend = " + digitHexes.Length);



            //digitHexes.Add(spawnedHex);
            //Debug.Log("SpawnNewDigit = " + spawnedHex + ". Count = " + digitHexes.Count);
            return true;
        }
        else
        {
            Debug.Log("SpawnNewDigit failed");
            return false;
        }
    }

}
