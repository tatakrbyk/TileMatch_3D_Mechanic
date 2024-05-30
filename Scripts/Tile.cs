using UnityEngine;

public class Tile : MonoBehaviour
{
    public string tileName;
    public Rigidbody tileRigidbody;
    public bool isClicked = false;

    private void Start()
    {
        tileRigidbody = GetComponent<Rigidbody>();
        tileRigidbody.isKinematic = true;
        tileRigidbody.freezeRotation = true;
    }

    private void OnMouseDown()
    {
        TileCheckEvent.onTileClicked?.Invoke(this);
    }

}
