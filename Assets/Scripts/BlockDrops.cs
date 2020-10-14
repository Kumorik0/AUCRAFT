using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDrops : MonoBehaviour
{



    void Update()
    {

    }

    [SerializeField]
    private string[] blockNames;


    public BlockStorage[] blockDrops;

    void Awake()
    {
        blockDrops = new BlockStorage[blockNames.Length];

        int newBlockID = 0;

        for (int i = 0; i < blockDrops.Length; i++)
        {
            blockDrops[newBlockID] = new BlockStorage(newBlockID, blockNames[i]);
            Debug.Log("Block: blockDrops[" + newBlockID + "] = " + blockNames[i]);
            newBlockID++;
        }
    }
}


public class BlockStorage
{
    public int blockID;
    public string blockName;

    public BlockStorage(int id, string myName)
    {
        blockID = id;
        blockName = myName;
    }
}