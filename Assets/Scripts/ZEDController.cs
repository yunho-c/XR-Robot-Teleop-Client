using UnityEngine;

public class ZEDController : MonoBehaviour
{
    [SerializeField] private GameObject ZEDFrameLeft;
    [SerializeField] private GameObject ZEDFrameRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleVideoStream(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void ToggleVideoStream(bool isOn)
    {
        if (isOn)
        {
            if (ZEDFrameLeft != null && ZEDFrameRight != null)
            {
                ZEDFrameLeft.SetActive(true);
                ZEDFrameRight.SetActive(true);
            }
        }
        else
        {
            if (ZEDFrameLeft != null && ZEDFrameRight != null)
            {
                ZEDFrameLeft.SetActive(false);
                ZEDFrameRight.SetActive(false);
            }
        }
    }
}
