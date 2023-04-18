using garden;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transball : MonoBehaviour
{
    public Vector3 position;

    public void setPosition(Vector3 transform)
    {
        Debug.Log("저장할 위치는 : " + transform);

        position = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (position == Vector3.zero) return;

            Quaternion rotation = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y + 180, gameObject.transform.rotation.z, gameObject.transform.rotation.w);

            //int characterIndex = other.gameObject.transform.root.GetComponent<ItemCharacterSwitcher>().currentCharacterIndex;
            //other.gameObject.transform.root.GetChild(characterIndex).GetChild(2).GetComponent<CharacterPuppet>().Respawn(position, rotation);
            other.gameObject.transform.root.GetChild(2).GetComponent<CharacterPuppet>().Respawn(position, rotation);
        }
    }
}
