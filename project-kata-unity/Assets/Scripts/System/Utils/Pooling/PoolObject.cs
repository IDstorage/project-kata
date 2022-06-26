using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly.Utils
{
    public class PoolObject : CustomBehaviour
    {

        public string uniqueName = "";

        public enum EState
        {
            PREPARE,
            USING
        }
        public EState state = EState.PREPARE;

        public void Init()
        {
            state = EState.PREPARE;
            gameObject.SetActive(false);
        }

        public PoolObject Use()
        {
            if (state == EState.USING) return null;
            state = EState.USING;
            gameObject.SetActive(true);
            return this;
        }

        public PoolObject Abandon()
        {
            if (state == EState.PREPARE) return null;
            state = EState.PREPARE;
            gameObject.SetActive(false);
            return this;
        }

    }
}