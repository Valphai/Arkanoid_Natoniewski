using GameSave;
using UnityEngine;

namespace GameLevel
{
    public class Generator : IPersistantObject
    {
        public static Generator Instance { get; private set; }
        [SerializeField] private SlotsFactory factory;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }
        public override void Save(GameDataWriter writer)
        {
            for (int i = 0; i < factory.Slots.Length; i++)
            {
                factory.Slots[i].Save(writer);
            }
        }
        public override void Load(GameDataReader reader)
        {
            for (int i = 0; i < factory.Slots.Length; i++)
            {
                factory.Slots[i].Load(reader);
            }
        }
    }   
}