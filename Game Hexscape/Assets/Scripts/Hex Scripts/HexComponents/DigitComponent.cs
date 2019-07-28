using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class DigitComponent : MonoBehaviour
{

    List<Hex> digitHexes;

    public int leadingZeroCount;
    public int numberToDisplay = 555;

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



    // Start is called before the first frame update
    void Start()
    {

        digitHexes = new List<Hex>();
        //digitHexes[0] = this.gameObject.GetComponent<Hex>();
        digitHexes.Add(this.gameObject.GetComponent<Hex>());
        InitialiseLeadingHexes();

        Hex owniingHex = this.gameObject.GetComponent<Hex>();
        owniingHex.onHexDig += DestroyDigitComponent; // Destroy(this);


        UpdateDisplayValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyDigitComponent()
    {
        Debug.Log("DestroyDigitComponent");
        Destroy(this);
    }


    private void UpdateDisplayValueNEW()
    {

    }

    private void UpdateDisplayValue()
    {
        Debug.Log("UpdateDisplayValue");

        int[] digitsToDisplay = ParseScore(1234);
        int index = 0; // index is used to itterate through the array of hex objects

        Queue<GameObject> digitObjectQueue = new Queue<GameObject>();

       // Debug.Log("digit hexes length  = " + digitHexes.Length);

        foreach (Hex hex in digitHexes)
        {

            if (hex == null) Debug.Log("Hex IS NUll");
            else
            {
                Debug.Log("Hex NOT NUll");
                Debug.Log(hex.gameObject.transform.position);
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

                Debug.Log("digitsToDisplay value at index "+ index  +" = " + digitsToDisplay[index]);
            }


            //NOTE! ADDING HEXES TO DIGITHEX LIST WILL ADD THEM IN THE WRONG ORDER WHEN THEY ARE REPLACED!
            //TODO: Custom 'insert' method for replacing hexes. May need to switch from list to array if possible.
            //TODO: Fix exception error when digitsToDisplay has a length of 1.
            //TODO: ...


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

    void InitialiseLeadingHexes()
    {
        Debug.Log("InitialiseLeadingHexes");
        for (int i = 0; i < leadingZeroCount; i++)
        //for (int i = 0; i < Mathf.Abs(leadingZeroCount); i++)
        {
            Debug.Log("leadingZeroCount current = " + i);
            if (!SpawnNewDigit()) break;
        }
    }

    private bool ReplaceDigit(int index, HexTypeEnum typeToDisplay)
    {
        Hex newHex = MapSpawner.Instance.SpawnHexAtLocation(MapSpawner.Instance.grid.WorldToCell(digitHexes[index].transform.position), typeToDisplay, true);
        digitHexes[index].DigHex();
        digitHexes[index] = newHex;
        return true;
    }

    //Spawns a new leading digit
    bool SpawnNewDigit(HexTypeEnum digitToAdd = HexTypeEnum.HexTile_Digit0)
    {
        Vector2Int nextHexCoord = MapSpawner.Instance.grid.WorldToCell(this.gameObject.transform.position) - new Vector2Int(digitHexes.Count, 0);

        Hex spawnedHex = MapSpawner.Instance.SpawnHexAtLocation(
            nextHexCoord,
            digitToAdd,
            false);

        if (spawnedHex)
        {
            digitHexes.Add(spawnedHex);

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
