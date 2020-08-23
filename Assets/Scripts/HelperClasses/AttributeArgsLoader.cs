using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;
using UnityEngine;


// Saves, loads and parses constructor arguments 
public class AttributeArgsLoader
{

    string databasePath = "Database/AttributeArgsDB";

    [System.Serializable]
    struct AttributeArgs {
        public string objectType;
        public string conditionType;
        public System.Object[] args;

        //TODO: Override operator to compare 
        public static bool operator ==(AttributeArgs x, AttributeArgs y)
        {
            return (x.objectType == y.objectType && x.conditionType == y.conditionType);
        }

        public static bool operator !=(AttributeArgs x, AttributeArgs y)
        {
            return (x.objectType != y.objectType || x.conditionType != y.conditionType);
        }



        public override bool Equals(object obj)
        {
            if (!(obj is AttributeArgs))
                return false;

            AttributeArgs mys = (AttributeArgs)obj;

            return (this.objectType == mys.objectType && this.conditionType == mys.conditionType);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }





    };


    // Loads the data for specified attribute type
    private AttributeArgs[] LoadArgumentData(string objectType)
    {
        TextAsset data = (TextAsset)Resources.Load(databasePath, typeof(TextAsset));


        List<AttributeArgs> foundArgs = JsonConvert.DeserializeObject<List<AttributeArgs>>(data.ToString(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
        return foundArgs != null ? foundArgs.ToArray() : null;

    }

    public void SaveAttributeArgs(string objectType, string conditionType, System.Object[] args)
    {
        AttributeArgs newArg;
        newArg.objectType = objectType;
        newArg.conditionType = conditionType;
        newArg.args = args;
        SaveArgsData(newArg);

     
    }

    private void SaveArgsData(AttributeArgs newArg)
    {
        // TODO: flter by object type
        // TODO: See if condition already exists
        // TODO: Replace old condition or add new condition - could we temprarily store line(s) location of of old data?
        //      Insert new argument data next to other object type data


        TextAsset data = (TextAsset)Resources.Load(databasePath, typeof(TextAsset));
        if (data != null)
        {
            List<AttributeArgs> foundArgs = JsonConvert.DeserializeObject<List<AttributeArgs>>(data.ToString(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
            if (foundArgs != null)
            {

                // Search for object and condition type in 
                int index = foundArgs.FindIndex(ind => ind.Equals(newArg));
                if (index > -1)
                {
                    foundArgs[index] = newArg;
                }
                else // Add new arg
                {
                    foundArgs.Add(newArg);
                    //TODO: Insert beside same objectType
                }

                foundArgs = SortArguments(foundArgs);

                JsonConvert.SerializeObject(foundArgs, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });
                string json = JsonConvert.SerializeObject(foundArgs, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });

                File.WriteAllText(Application.dataPath + "/Resources/" + databasePath + ".json", json);
            }
            else
            {

                List<AttributeArgs> newArgs = new List<AttributeArgs>();
                newArgs.Add(newArg);

                string json = JsonConvert.SerializeObject(newArgs, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Objects });

                File.WriteAllText(Application.dataPath + "/Resources/" + databasePath + ".json", json);
            }
        }

    }

    List<AttributeArgs> SortArguments(List<AttributeArgs> argumentsToSort)
    {
        List<AttributeArgs> returnArgs = argumentsToSort.OrderBy(x => x.objectType).ThenBy(x => x.conditionType).ToList();
        return returnArgs;
    }

    public System.Object[] GetArgsForAttribute(string objectType/*, string conditionType*/)
    {

        AttributeArgs[] foundArgs = LoadArgumentData(objectType);

        if (foundArgs != null)
        {

            AttributeArgs argsToFind = new AttributeArgs();
            argsToFind.objectType = objectType;
            //argsToFind.conditionType = conditionType;

            foreach (AttributeArgs foundArg in foundArgs)
                if (argsToFind == foundArg)
                {

                    return foundArg.args;
                }

        }


        return null;
    }

}
