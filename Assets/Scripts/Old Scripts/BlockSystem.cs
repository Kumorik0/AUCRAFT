using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSystem : MonoBehaviour
{

    // Arrays for all solid blocks
    // The two arrays need to match up

    [SerializeField]
    private Sprite[] solidBlocks;
    [SerializeField]
    private string[] solidNames;

    [SerializeField]
    private Sprite[] backingBlocks;
    [SerializeField]
    private string[] backingNames;

    // Array to store all blocks created in Awake()
    public Block[] allBlocks;

    private void Awake()
    {
        // Initialize allBlocks array
        allBlocks = new Block[solidBlocks.Length + backingBlocks.Length];

        // Temp int to store block ID as we go
        int newBlockID = 0;

        // For loops to populate main allBlocks array
        for (int i = 0; i < solidBlocks.Length; i++)
        {
            allBlocks[newBlockID] = new Block(newBlockID, solidNames[i], solidBlocks[i], true);
            Debug.Log("Solid block: allBlocks[" + newBlockID + "] = " + solidNames[i]);
            newBlockID++;
        }

        for (int i = 0; i < backingBlocks.Length; i++)
        {
            allBlocks[newBlockID] = new Block(newBlockID, backingNames[i], backingBlocks[i], false);
            Debug.Log("Backing block: allBlocks[" + newBlockID + "] = " + backingNames[i]);
            newBlockID++;
        }
    }

}

public class Block
{
    public int blockID;
    public string blockName;
    public Sprite blockSprite;
    public bool isSolid;

    public Block(int id, string myName, Sprite mySprite, bool amISolid)
    {
        blockID = id;
        blockName = myName;
        blockSprite = mySprite;
        isSolid = amISolid;
    }
}
