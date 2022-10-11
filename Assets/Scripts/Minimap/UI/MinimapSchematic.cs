using GameStudio.HunterGatherer.Environment.Flood;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace GameStudio.HunterGatherer.Minimap
{
    /// <summary>Handle and create a schematic minimap during play</summary>
    [RequireComponent(typeof(RawImage)), RequireComponent(typeof(MinimapReferences)), RequireComponent(typeof(RectTransform))]
    public class MinimapSchematic : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private Color[] heightColors = null;

        [SerializeField]
        private Color impassableTerrain = new Color(0, 0, 0, 1);

        [SerializeField]
        private int[] cameraLayerSizes = null;

        [SerializeField]
        private int waitForFrames = 2;

        [SerializeField]
        private float resizeSpeed = 2;

        [Header("References")]
        [SerializeField]
        private Texture creationTexture = null;

        [SerializeField]
        private RawImage tweenTexture = null;

        [SerializeField]
        private GameObject floodObject = null;

        [SerializeField]
        private NavMeshAgent unitNavMeshAgent = null;

        private Color[,] minimapColorData;
        private RawImage minimapRawImage;
        private MinimapReferences references;
        private RectTransform tweenTransform;
        private RectTransform minimapTransform;
        private Flood flood;
        private float[,] heightData;
        private float heightAvoidance;

        private void Awake()
        {
            minimapRawImage = GetComponent<RawImage>();
            minimapTransform = GetComponent<RectTransform>();
            tweenTransform = tweenTexture.GetComponent<RectTransform>();
            references = GetComponent<MinimapReferences>();
            flood = floodObject.GetComponent<Flood>();

            if (tweenTransform == null)
            {
                Debug.LogError("The variable tweenTransform in MinimapSchematic doesn't have a value assigned!");
            }
        }

        private void Start()
        {
            StartCoroutine(DelaySchematicMinimapCreation(waitForFrames, creationTexture));
        }

        /// <summary>Call the UpdateSchematicMinimap function with a delay of a given amount of frames</summary>
        private IEnumerator DelaySchematicMinimapCreation(int frames, Texture texture)
        {
            references.DepthCamera.gameObject.SetActive(true);

            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            heightAvoidance = unitNavMeshAgent.height;

            CreateColorArray(GetTexture2D(texture));
            minimapRawImage.texture = CreateTexture(minimapColorData);

            references.DepthCamera.gameObject.SetActive(false);
        }

        /// <summary>Create a color array out of a texture</summary>
        private void CreateColorArray(Texture2D texture)
        {
            minimapColorData = new Color[texture.width, texture.height];
            float currentHeight;

            UpdateHeightData(texture);

            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    currentHeight = heightData[i, j];

                    if (flood != null)
                    {
                        if ((int)heightData[i, j] <= flood.CurrentHeight)
                        {
                            minimapColorData[i, j] = heightColors[0];
                            continue;
                        }
                    }
                    else
                    {
                        if ((int)heightData[i, j] <= floodObject.transform.position.y)
                        {
                            minimapColorData[i, j] = heightColors[0];
                            continue;
                        }
                        else
                        {
                            minimapColorData[i, j] = heightColors[1];
                        }
                    }

                    if ((i < texture.width - 1 && (currentHeight - heightData[i + 1, j] >= heightAvoidance || currentHeight - heightData[i + 1, j] <= -heightAvoidance)) ||
                        (j < texture.height - 1 && (currentHeight - heightData[i, j + 1] >= heightAvoidance || currentHeight - heightData[i, j + 1] <= -heightAvoidance)) ||
                        (i > 0 && (currentHeight - heightData[i - 1, j] >= heightAvoidance || currentHeight - heightData[i - 1, j] <= -heightAvoidance)) ||
                        (j > 0 && (currentHeight - heightData[i, j - 1] >= heightAvoidance || currentHeight - heightData[i, j - 1] <= -heightAvoidance)))
                    {
                        minimapColorData[i, j] = impassableTerrain;
                    }
                    else
                    {
                        if (flood != null)
                        {
                            for (int k = 0; k < flood.HeightsOfSections.Count; k++)
                            {
                                if ((int)currentHeight <= flood.HeightsOfSections[k])
                                {
                                    minimapColorData[i, j] = heightColors[k];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Create a texture out of a color array</summary>
        private Texture CreateTexture(Color[,] colorArray)
        {
            Texture2D minimapUpdateData = new Texture2D(colorArray.GetLength(0), colorArray.GetLength(1));

            for (int i = 0; i < colorArray.GetLength(0); i++)
            {
                for (int j = 0; j < colorArray.GetLength(1); j++)
                {
                    minimapUpdateData.SetPixel(i, j, colorArray[i, j]);
                }
            }
            minimapUpdateData.Apply();
            return minimapUpdateData;
        }

        /// <summary>Convert the data of the lowest terrain into water, update the flooded terrain and apply the texture</summary>
        public void UpdateMinimapFlood(Texture texture)
        {
            if (flood.CurrentSection < cameraLayerSizes.Length)
            {
                StartCoroutine(UpdateFloodAndResizeMinimap(texture, waitForFrames));
            }
        }

        ///<summary>Update the flood on the minimap and slowly resize it to the right size</summary>
        private IEnumerator UpdateFloodAndResizeMinimap(Texture texture, int frames)
        {
            // Resize depth camera vision and set it active
            references.DepthCamera.gameObject.SetActive(true);
            references.DepthCamera.orthographicSize = cameraLayerSizes[flood.CurrentSection];

            // Wait for the depth camera to render properly
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            // Create the schematic data and update it base on what will get flooded
            CreateColorArray(GetTexture2D(texture));

            minimapRawImage.texture = CreateTexture(minimapColorData);

            // Deactivate depth camera for performance
            references.DepthCamera.gameObject.SetActive(false);

            // If the flood hasn't taken over the entire map, shrink the minimap
            Color[,] waterMap = new Color[,]
            {
                { heightColors[0] }
            };
            
            // Fill the background with a water color and recreate the minimap for a texture we will resize
            minimapRawImage.texture = CreateTexture(waterMap);
            tweenTexture.texture = CreateTexture(minimapColorData);

            // Resize this texture to the size it originally represented
            tweenTransform.sizeDelta = new Vector2(
                (float)cameraLayerSizes[flood.CurrentSection] / cameraLayerSizes[flood.CurrentSection - 1] * tweenTransform.sizeDelta.x,
                (float)cameraLayerSizes[flood.CurrentSection] / cameraLayerSizes[flood.CurrentSection - 1] * tweenTransform.sizeDelta.y);

            // Set the texture active
            tweenTexture.gameObject.SetActive(true);

            LeanTween.value(tweenTransform.sizeDelta.x, minimapTransform.sizeDelta.x, resizeSpeed)
                .setOnUpdate(val => tweenTransform.sizeDelta = new Vector2(val, val))
                .setEase(LeanTweenType.easeInOutCubic);

            yield return new WaitForSeconds(resizeSpeed);
                    
            tweenTexture.gameObject.SetActive(false);

            // Set the texture back to the original minimap image
            minimapRawImage.texture = CreateTexture(minimapColorData); 
        }

        private void UpdateHeightData(Texture2D texture)
        {
            heightData = new float[texture.width, texture.height];

            for (int i = 0; i < texture.width; i++)
            {
                for (int j = 0; j < texture.height; j++)
                {
                    heightData[i, j] = texture.GetPixel(i, j).r * references.DepthCamera.farClipPlane + (references.DepthCamera.transform.position.y - references.DepthCamera.farClipPlane);
                }
            }
        }

        private Texture2D GetTexture2D(Texture texture)
        {
            if (texture is Texture2D)
            {
                return texture as Texture2D;
            }
            if (texture is RenderTexture)
            {
                return ConvertRenderTexture(texture as RenderTexture);
            }
            Debug.LogError("Cannot return given texture as Texture2D");
            return null;
        }


        ///<summary>Convert a render texture into a texture 2D</summary>
        private Texture2D ConvertRenderTexture(RenderTexture renderTexture)
        {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture as RenderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            return texture;
        }
    }
}