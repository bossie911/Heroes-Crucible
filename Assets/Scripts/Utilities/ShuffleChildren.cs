using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class shuffles the positions of all of a GameObject's children.
/// </summary>
public class ShuffleChildren : MonoBehaviour
{
    public Vector3[] children;
    // Start is called before the first frame update
    void Awake()
    {
        children = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).position;
        }

    }
    /// <summary>
    /// Method that shuffles an array using the Knuth shuffle algorithm.
    /// </summary>
    /// <param name="array"> The array of Transforms that you wish to shuffle.</param>
    public void ShuffleTransform()
    {
        for (int i = 0; i < children.Length; i++)
        {
            int random = Random.Range(i, children.Length);
            Vector3 temp = children[i];
            children[i] = children[random];
            children[random] = temp;
        }
        ApplyPosition(children);
    }

    /// <summary>
    /// Method that saves the locations of all transforms in an array,
    /// and swaps the positions of children with the values found in the Transform array.
    /// </summary>
    /// <param name="array"> The array of Transforms that you wish you swap positions of.</param>
    void ApplyPosition(Vector3[] array)
    {

        Vector3[] snapshot = new Vector3[array.Length];

        //Fill array with snapshots of positions which don't update when swapping spawnpoints
        for (int i = 0; i < array.Length; i++)
        {
            snapshot[i] = array[i];
        }

        //Swap positions of spawnpoints with the new positions
        for (int i = 0; i < array.Length; i++)
        {
            transform.GetChild(i).position = snapshot[i];
        }
    }

}
