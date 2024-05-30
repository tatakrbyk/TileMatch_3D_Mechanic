using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private WorldUISlot[] slotPos;
    [SerializeField] private SO_LevelData[] _levelDatas;
    [SerializeField] private UIManager _uiManager;

    private float XOffset = 18f;
    private float YOffset = 20f;

    private int totalTile;
    private bool isMoving = false;

    private string[] tilePrefabs = new string[7]
    {
        "cTex_1",
        "cTex_2",
        "cTex_3",
        "cTex_4",
        "cTex_5",
        "cTex_6",
        "cTex_7"
    };

    private void OnEnable()
    {
        TileCheckEvent.onTileClicked += ClickAction;
    }
    private void OnDisable()
    {
        TileCheckEvent.onTileClicked -= ClickAction;
    }


    private void ClickAction(Tile tile)
    {
        if (isMoving)
            return;

        isMoving = true;
        for (int i = 0; i < slotPos.Length; ++i)
        {
            if (slotPos[i].isEmpty)
            {
                if (i > 0)
                {
                    if (!slotPos[i - 1].occupiedTile.tileName.Equals(tile.tileName))
                    {
                        for (int j = i - 1; j >= 0; --j)
                        {
                            if (slotPos[j].occupiedTile.tileName.Equals(tile.tileName)) // it moves backwards and looks for tiles of the same type.
                            {
                                int numberOfStep = Math.Abs(i - j) - 1; // The number of steps the same type will take to reach its side slot
                                for (int k = 1; k <= numberOfStep; ++k)
                                {
                                    // other type 1 slot shift right 
                                    slotPos[i - k].occupiedTile.transform.DOMove(
                                        new Vector3(slotPos[i - k + 1].transform.position.x, slotPos[i - k + 1].transform.position.y, slotPos[i - k + 1].transform.position.z), 
                                        0.2f);
                                    slotPos[i - k + 1].occupiedTile = slotPos[i - k].occupiedTile;
                                    slotPos[i - k + 1].isEmpty = false;
                                    if (k == numberOfStep)
                                    {
                                        // The clicked tile is placed in the side slot of the tile of the same type
                                        tile.transform.rotation = new Quaternion(0, 0, 0, 0);
                                        tile.GetComponent<BoxCollider>().enabled = false;
                                        tile.transform.DOMove(new Vector3(slotPos[i - k].transform.position.x, slotPos[i - k].transform.position.y, slotPos[i - k].transform.position.z), 0.1f).OnComplete(
                                            () =>
                                            {
                                                DOVirtual.DelayedCall(0.25f, () =>
                                                {
                                                    isMoving = false;
                                                });
                                            });
                                        slotPos[i - k].occupiedTile = tile.GetComponent<Tile>();
                                        slotPos[i - k].isEmpty = false;
                                        if (MatchCheck(i - k))
                                        {
                                            MatchLogic(i - k);
                                            DOVirtual.DelayedCall(0.4f, () => { TileShift(i - k + 1); });
                                        }
                                        else
                                        {
                                            if (CheckLose())
                                            {
                                                DOVirtual.DelayedCall(0.1f, () =>
                                                {
                                                    _uiManager.Show(GameManager.GameState.Lose);
                                                });

                                            }
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                tile.transform.rotation = new Quaternion(0, 0, 0, 0);
                tile.GetComponent<BoxCollider>().enabled = false;
                tile.transform.DOMove(
                    new Vector3(slotPos[i].transform.position.x, slotPos[i].transform.position.y,
                        slotPos[i].transform.position.z), 0.1f).OnComplete(() =>
                        {
                            DOVirtual.DelayedCall(0.25f, () =>
                            {
                                isMoving = false;
                            });
                        });
                slotPos[i].GetComponent<WorldUISlot>().occupiedTile = tile.GetComponent<Tile>();
                slotPos[i].isEmpty = false;
                if (MatchCheck(i))
                {
                    MatchLogic(i);
                }
                else
                {
                    if (CheckLose())
                    {
                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            _uiManager.Show(GameManager.GameState.Lose);
                        });

                    }
                }
                break;
            }
        }
    }
    private void MatchLogic(int index)
        {
        DOVirtual.DelayedCall(0.2f, () =>
        {
            MatchAnim(index);
            DOVirtual.DelayedCall(0.1f, () =>
            {
                Destroy(slotPos[index].occupiedTile.gameObject);
                slotPos[index].occupiedTile = null;
                slotPos[index].isEmpty = true;

                Destroy(slotPos[index - 1].occupiedTile.gameObject);
                slotPos[index - 1].occupiedTile = null;
                slotPos[index - 1].isEmpty = true;

                Destroy(slotPos[index - 2].occupiedTile.gameObject);
                slotPos[index - 2].occupiedTile = null;
                slotPos[index - 2].isEmpty = true;
            });
        });

        totalTile -= 3;
        if(totalTile <= 0)
        {
            _uiManager.Show(GameManager.GameState.NextGame);
        }
    }
    private void TileShift(int currIndex)
    {
        if (currIndex >= 3)
        {
            for (int i = currIndex; i < slotPos.Length; ++i)
            {
                if (!slotPos[i].isEmpty)
                {
                    slotPos[i].occupiedTile.transform
                        .DOMove(
                            new Vector3(slotPos[i - 3].transform.position.x,
                                slotPos[i - 3].transform.position.y, slotPos[i - 3].transform.position.z), 0.2f).OnComplete(
                            () =>
                            {
                                DOVirtual.DelayedCall(0.3f, () =>
                                {
                                    isMoving = false;
                                });

                            });
                    slotPos[i - 3].occupiedTile = slotPos[i].occupiedTile;
                    slotPos[i - 3].isEmpty = false;
                    slotPos[i].occupiedTile = null;
                    slotPos[i].isEmpty = true;
                }
            }
        }
    }
    private bool CheckLose()
    {
        for (int i = 0; i < slotPos.Length; ++i)
        {
            if (slotPos[i].isEmpty)
                return false;
        }
        return true;
    }
    private bool MatchCheck(int currIndex)
    {
        if (currIndex >= 2)
        {
            if (slotPos[currIndex].occupiedTile.tileName.Equals(slotPos[currIndex - 1].occupiedTile.tileName)
                &&
                slotPos[currIndex].occupiedTile.tileName.Equals(slotPos[currIndex - 2].occupiedTile.tileName))
            {
                return true;
            }
            return false;
        }

        return false;
    }

    private void MatchAnim(int currIndex)
    {
        slotPos[currIndex].occupiedTile.transform.DOScale(0, 0.1f).OnStart(() =>
        {
            slotPos[currIndex - 1].occupiedTile.transform.DOScale(0, 0.1f).OnStart(() =>
            {
                slotPos[currIndex - 2].occupiedTile.transform.DOScale(0, 0.1f);
            });
        });
    }

    public void ClearSlot()
    {
        for(int i = 0; i < slotPos.Length; ++i)
        {
            if (slotPos[i].occupiedTile != null)
            {
                Destroy(slotPos[i].occupiedTile.gameObject);
                slotPos[i].occupiedTile = null;
                slotPos[i].isEmpty = true;
            }
        }
    }
    public void GenerateTile(int levelID)
    {
        isMoving = false;
        if (levelID > _levelDatas.Length) return; // Check Leveldata size

        SO_LevelData newLevel = new SO_LevelData();
        foreach(var data in _levelDatas)
        {
            if (levelID == data.levelID)
                newLevel = data;
        }

        // Random Place
        for(int i = 0; i < tilePrefabs.Length; ++i)
        {
            for(int j = 0; j < newLevel.tileAmount; ++j)
            {
                GameObject obj = Instantiate(
                    Resources.Load<GameObject>("Tile/" + tilePrefabs[i]), 
                    new Vector3(UnityEngine.Random.Range(-XOffset, XOffset), UnityEngine.Random.Range(1f, YOffset), UnityEngine.Random.Range(-5f, 15f)),
                    new Quaternion(0, 0, 0, 0), 
                    transform
                    );
                obj.GetComponent<Tile>().tileName = tilePrefabs[i];
            }
        }

        totalTile = newLevel.tileAmount * (tilePrefabs.Length) ;
        //Debug.Log(totalTile); 
    }
}
