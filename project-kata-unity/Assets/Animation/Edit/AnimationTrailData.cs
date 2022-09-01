using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AnimationTrailData", fileName = "NewAnimationTrailData")]
public class AnimationTrailData : ScriptableObject
{
    [System.Serializable]
    public struct Box
    {
        public SerializableVector3 start, end;
        public float width, height;
    }

    [System.Serializable]
    public struct Track
    {
        public float start;
        public float end;
        public Box[] boxes;
    }

    public List<Track> tracks;


    public bool TrySet(int idx, Track data)
    {
        if (tracks == null) tracks = new List<Track>();

        if (tracks.Count < idx) return false;
        if (tracks.Count == idx)
        {
            tracks.Add(data);
            return true;
        }
        tracks[idx] = data;
        return true;
    }
}
