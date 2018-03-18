using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{

    public string characterName = "apollo";
    public Sprite[] characterPoses = null;
    public string[] characterPosesNames = null;
    Sprite sprite;

    public Sprite LoadSprite(string spriteName)
    {
        string finalPath;
        WWW localFile;
        Texture texture;

        finalPath = "file://" + Application.streamingAssetsPath + "/char/" + characterName + "/(a)" + characterName + spriteName + ".png";
        localFile = new WWW(finalPath);
        Debug.Log(localFile);

        texture = localFile.texture;
        sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return sprite;
    }

    // Use this for initialization
    void Start()
    {
        int characterPoseCount = 0;
        foreach (string s in characterPosesNames)
        {
            Debug.Log("file://" + Application.streamingAssetsPath + "/char/" + characterName + "/(a)" + characterName + s + ".png");
            Debug.Log(s);
            characterPoseCount++;
            Debug.Log(characterPoseCount);
            sprite = LoadSprite(s);
            characterPoses[characterPoseCount] = sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}