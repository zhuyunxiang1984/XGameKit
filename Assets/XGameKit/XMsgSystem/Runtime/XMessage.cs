using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public abstract class XMessage : IXPoolable
    {
        public string name { get; protected set; }

        public void SetName(string name)
        {
            this.name = name;
        }

        public abstract void Reset();
    }
}