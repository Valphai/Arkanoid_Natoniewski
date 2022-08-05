using UnityEngine;

namespace GameSave
{
    public abstract class IPersistantObject : MonoBehaviour
    {
        public virtual void Save(GameDataWriter writer)
        {
            writer.Write(transform.position);
        }
        public virtual void Load(GameDataReader reader)
        {
            transform.position = reader.ReadVector3();
        }
    }
}