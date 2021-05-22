using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerupable : MonoBehaviour {
    [System.Serializable]
    public abstract class PowerupableData {
        public virtual void Add(PowerupableData d) {
            Check();
        }
        public virtual void Mul(PowerupableData d) {
            Check();
        }
        public virtual void Set(PowerupableData d) {
            Check();
        }
        public abstract void Check();
        public abstract PowerupableData Clone();
    }

    private PowerupableData _data;
    public PowerupableData data{ get {return _data;} }

    public virtual void SetData(PowerupableData d) {
        _data = d.Clone();
    }
}
