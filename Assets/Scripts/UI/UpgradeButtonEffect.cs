using UnityEngine;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.UI
{
    /// <summary>Adds a horizontal scroling or pulse effect to the upgrade buttons by manipulating the alpha channel.</summary>

    public class UpgradeButtonEffect : MonoBehaviour
    {
        Image background;
        [SerializeField] int pos = 1; //The order of this button from left to right. It should be between 1 and numberOfButtons.
        [SerializeField] int numberOfButtons = 3;
        [SerializeField] float delta = 0.6f;
        [SerializeField] float speed = 7f;
        [SerializeField] bool horizontalScroll = true;
        [SerializeField] bool leftToRight;
        float offset = 0;

        // Start is called before the first frame update
        void Start()
        {
            background = GetComponent<Image>();
            if (!background)
            {
                Debug.LogWarning("Missing image component.");
                return;
            }

            //Set an offset to create the horizontal scrolling effect.
            if (horizontalScroll)
            {
                offset = pos / (numberOfButtons * delta);
                if (leftToRight)
                    offset = 1 - offset;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!background)
                return;

            //Apply the effect.
            Color c = background.color;
            c.a = (Mathf.Cos(Time.time * speed + offset) + 1) / 2;
            background.color = c;

        }
    }
}
