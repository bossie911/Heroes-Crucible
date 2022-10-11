using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using FMOD.Studio;

namespace GameStudio.HunterGatherer.Audio
{

    [System.Serializable]
    public class AudioParameter
    {
        public string name;
        public float value;

        public AudioParameter() { }
        public AudioParameter(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    public static class Extensions
    {
        public static List<AudioParameter> ToAudioParameters(this ParamRef[] self)
        {
            return self.ToList().Select(x => new AudioParameter(x.Name, x.Value)).ToList();
        }

        public static ParamRef GetParam(this ParamRef[] self, AudioParameter param)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Name == param.name)
                {
                    return self[i];
                }
            }

            return null;
        }

        public static ParamRef GetParam(this ParamRef[] self, string name)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Name == name)
                {
                    return self[i];
                }
            }

            return null;
        }

        public static int GetParamIndex(this ParamRef[] self, ParamRef param)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Name == param.Name)
                {
                    return i;
                }
            }

            return -1;
        }

        public static int GetParamIndex(this ParamRef[] self, string name)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool Contains(this ParamRef[] self, PARAMETER_ID param)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].ID.Equals(param))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this ParamRef[] self, AudioParameter param)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Name == param.name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this ParamRef[] self, string name)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this List<AudioParameter> self, string name)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (self[i].name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this List<AudioParameter> self, ParamRef param)
        {
            for (int i = 0; i < self.Count; i++)
            {
                if (self[i].name == param.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this AudioParameter[] self, string name)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this AudioParameter[] self, ParamRef param)
        {
            for (int i = 0; i < self.Length; i++)
            {
                if (self[i].name == param.Name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}