using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CollectibleComponent : BaseHexComponent, IChargeable
{

    GameObject spawnedCollectible = null;

    const string CollectiblesAssetPath = "Prefabs/Collectibles/";

    Vector3 offset = new Vector3(0,0.5f, 0);

    private string objectToSpawnName
    {
        get { return objectToSpawnName; }
        set { if (objectToSpawnName == null) objectToSpawnName = value; }
    }


    public void SpawnCollectible(string objectToSpawnName)
    {
        Object loadedObject = Resources.Load(CollectiblesAssetPath + objectToSpawnName);
        if (loadedObject)
            spawnedCollectible = GameObject.Instantiate(loadedObject, this.transform.position + offset, this.transform.rotation) as GameObject;
    }

    public void DestroyCollectible()
    {
#if (UNITY_EDITOR)
        if (spawnedCollectible) DestroyImmediate(spawnedCollectible); // Needed for using the level Editor Window
#endif
        if (spawnedCollectible) GameObject.Destroy(spawnedCollectible);
    }

    public override void CleanupComponent()
    {
        DestroyCollectible();
    }



    EChargeType defaultCharge;
    EChargeType currentCharge;
    public EChargeType chargeType { get => currentCharge; }
    public bool isSource { get => false; }
    public int chargeValue { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void ReceieveCharge(EChargeType chargeTypeReceived)
    {
        if (chargeType == chargeTypeReceived)
        SpawnCollectible(objectToSpawnName); 
    }

    public void RemoveCharge()
    {
        DestroyCollectible();
    }

    public List<IChargeable> GetNeighbourChargeInterfaces()
    {
        return null;
     //   throw new System.NotImplementedException();
    }

    public EChargeType RequestCharge()
    {
        throw new System.NotImplementedException();
    }
}
