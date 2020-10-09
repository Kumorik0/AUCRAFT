using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{

    // Refernce to the BlockSystem script
    private BlockSystem blockSys;

    // Variables to hold data regarding currect block type
    private int currentBlockID = 0;
    private Block currentBlock;

    private int selectableBlocksTotal;

    // Variables for the block template
    private GameObject blockTemplate;
    private SpriteRenderer currentRend;

    // Bools to control building system
    private bool buildModeOn = false;
    private bool buildBlocked = false;

    // Float to adjust the size of blocks when placing in world
    [SerializeField]
    private float blockSizeMod;

    // Layer masks to control raycasting
    [SerializeField]
    private LayerMask solidNoBuildLayer;
    [SerializeField]
    private LayerMask backingNoBuildLayer;
    [SerializeField]
    private LayerMask allBlocksLayer;

    // Reference to the player object
    private GameObject playerObject;

    [SerializeField]
    private float maxBuildDistance;

    private void Awake()
    {
        // Store reference to block system script
        blockSys = GetComponent<BlockSystem>();

        // Find player and store reference
        playerObject = GameObject.Find("PlayerCharacter [placeholder]");

    }

    private void Update()
    {
        // If E key pressed, toggle build mode.
        if (Input.GetKeyDown("e"))
        {
            // Flip bool
            buildModeOn = !buildModeOn;

            // If we have a current template, destroy it
            if (blockTemplate != null)
            {
                Destroy(blockTemplate);
            }

            // If we don't have a current block type set
            if (currentBlock == null)
            {
                // Ensure allBlocks array is ready
                if (blockSys.allBlocks[currentBlockID] != null)
                {
                    // Get a new currentBlock using the ID variable
                    currentBlock = blockSys.allBlocks[currentBlockID];
                }
            }

            if (buildModeOn)
            {
                // Create a new object for blockTemplate
                blockTemplate = new GameObject("CurrentBlockTemplate");
                // Add and store reference to a SpriteRenderer on the template object
                currentRend = blockTemplate.AddComponent<SpriteRenderer>();
                // Set the sprite of the template object to match current block type
                currentRend.sprite = currentBlock.blockSprite;
            }
        }

        if (buildModeOn && blockTemplate != null)
        {
            float newPosX = Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / blockSizeMod) * blockSizeMod;
            float newPosY = Mathf.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / blockSizeMod) * blockSizeMod;
            blockTemplate.transform.position = new Vector2(newPosX, newPosY);

            RaycastHit2D rayHit;

            if (currentBlock.isSolid == true)
            {
                rayHit = Physics2D.Raycast(blockTemplate.transform.position, Vector2.zero, Mathf.Infinity, solidNoBuildLayer);
            }
            else
            {
                rayHit = Physics2D.Raycast(blockTemplate.transform.position, Vector2.zero, Mathf.Infinity, backingNoBuildLayer);
            }

            if (rayHit.collider != null)
            {
                buildBlocked = true;
            }
            else
            {
                buildBlocked = false;
            }

            if (Vector2.Distance(playerObject.transform.position, blockTemplate.transform.position) > maxBuildDistance)
            {
                buildBlocked = true;
            }

            if (buildBlocked)
            {
                currentRend.color = new Color(1f, 0f, 0f, 1f);
            }
            else
            {
                currentRend.color = new Color(1f, 1f, 1f, 1f);
            }

            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            if (mouseWheel != 0)
            {
                selectableBlocksTotal = blockSys.allBlocks.Length - 1;

                if (mouseWheel > 0)
                {
                    currentBlockID--;

                    if (currentBlockID < 0)
                    {
                        currentBlockID = selectableBlocksTotal;
                    }
                }
                else if (mouseWheel < 0)
                {
                    currentBlockID++;

                    if (currentBlockID >selectableBlocksTotal)
                    {
                        currentBlockID = 0;
                    }
                }

                currentBlock = blockSys.allBlocks[currentBlockID];
                currentRend.sprite = currentBlock.blockSprite;
            }

            if (Input.GetMouseButtonDown(0) && buildBlocked == false)
            {
                GameObject newBlock = new GameObject(currentBlock.blockName);
                newBlock.transform.position = blockTemplate.transform.position;
                SpriteRenderer newRend = newBlock.AddComponent<SpriteRenderer>();
                newRend.sprite = currentBlock.blockSprite;

                if (currentBlock.isSolid == true)
                {
                    newBlock.AddComponent<BoxCollider2D>();
                    newBlock.layer = 9;
                    newRend.sortingOrder = -10;
                }
                else
                {
                    newBlock.AddComponent<BoxCollider2D>();
                    newBlock.layer = 11;
                    newRend.sortingOrder = -15;
                }
            }

            if (Input.GetMouseButtonDown(1) && blockTemplate != null)
            {
                RaycastHit2D destroyHit = Physics2D.Raycast(blockTemplate.transform.position, Vector2.zero, Mathf.Infinity, allBlocksLayer);

                if (destroyHit.collider != null)
                {
                    Destroy(destroyHit.collider.gameObject);
                }
            }
        }
    }

}
