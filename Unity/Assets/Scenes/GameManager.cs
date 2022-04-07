using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player player;
    public GameObject cameraSet;
    public GameObject[] warp;

    public Image saveImage;
    public Text saveText;

    private bool b_Saveing;

    void Start()
    {

    }

    void Update()
    {

    }

    public void WarpSave(int num)
    {
        PlayerPrefs.SetInt("Warp", num);
        if (!b_Saveing)
        {
            b_Saveing = true;
            StartCoroutine(SaveImageEff());
        }
    }

    IEnumerator SaveImageEff()
    {
        saveImage.color = new Vector4(1, 1, 1, 0);
        saveText.color = new Vector4(1, 1, 1, 0);
        saveImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        for(float i = 0; i < 0.5f; i+=0.01f)
        {
            saveImage.color = new Vector4(1, 1, 1, i);
            saveText.color = new Vector4(1, 1, 1, i * 2);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2);

        for (float i = 0.5f; i > 0; i -= 0.01f)
        {
            saveImage.color = new Vector4(1, 1, 1, i);
            saveText.color = new Vector4(1, 1, 1, i * 2);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.2f);

        b_Saveing = false;
        saveImage.gameObject.SetActive(false);
    }

    public void WarpLoad(string warp)
    {
        player.transform.position = this.warp[PlayerPrefs.GetInt("Warp")].transform.position;
        player.GetComponent<SpriteRenderer>().sortingLayerName = warp;
    }
}
