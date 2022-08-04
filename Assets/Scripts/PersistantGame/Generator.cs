using System.Collections.Generic;
using GameLevel;
using GameLevel.Bricks;
using GameSave;
using UnityEngine;

namespace PersistantGame
{
    public class Generator : IPersistantObject
    {
        private int seed;
        private BrickSlot[] slots;
        private List<Brick> activeBricks;
        [SerializeField] private SlotsFactory factory;

        private void Awake()
        {
            seed = Random.Range(0, 10000);
        }
        public override void Save(GameDataWriter writer)
        {
            writer.Write(seed);
            for (int i = 0; i < slots.Length; i++)
            {
                writer.Write(slots[i].IsFilled() ? 1 : 0);
            }
        }
        public override void Load(GameDataReader reader)
        {
            seed = reader.ReadInt();
            for (int i = 0; i < slots.Length; i++)
            {
                if (reader.ReadInt() == 1)
                    factory.PutBrickInto(slots[i]);
            }
        }
        public void StartNewGame()
        {
            if (slots == null) factory.SpawnSlots(ref slots);
            if (activeBricks == null) activeBricks = new List<Brick>();

            for (int i = 0; i < activeBricks.Count; i++)
            {
                factory.ReturnBrick(activeBricks[i]);
            }

            GenerateMap();
        }

        private void GenerateMap()
        {
            // int symmetrical = Random.Range(0, 2);
            // if (symmetrical == 1)
            // {
                
            // }
            if (true) // one big shape
            {
                
            }
            else if () // 3 shapes
            else // irregular
            {

            }
        }
        /// <summary>
        /// Go in line from a starting point
        /// </summary>
        /// <param name="nTimes">How many times to go in this direction</param>
        /// <param name="start">Starting slot</param>
        private void GoIn(Direction dir, int nTimes, BrickSlot start)
        {
            factory.PutBrickInto(start);
            BrickSlot next = start.NextIn(dir);
            for (int i = 0; i < nTimes; i++)
            {
                factory.PutBrickInto(next);
                next = next.NextIn(dir);
            }
        }
        private void RandomFillGoIn(Direction dir, int nTimes, BrickSlot start)
        {
            factory.PutBrickInto(start);
            BrickSlot next = start.NextIn(dir);
            for (int i = 0; i < nTimes; i++)
            {
                float dice = Random.Range(0, 2);
                if (dice == 1)
                    factory.PutBrickInto(next);

                next = next.NextIn(dir);
            }
        }
        private void RandomFillDirectionGoIn(Direction dir, int nTimes, BrickSlot start)
        {
            BrickSlot[] beenTo = new BrickSlot[nTimes + 1];
            beenTo[0] = start;

            factory.PutBrickInto(start);
            BrickSlot next = start.NextIn(dir);
            for (int i = 0; i < nTimes; i++)
            {
                float dice = Random.Range(0, 2);
                if (dice == 1)
                {
                    factory.PutBrickInto(next);
                    beenTo[i + 1] = next;
                }

                Direction[] dirFan = dir.Fan();
                int randDir = Random.Range(0, dirFan.Length);

                next = next.NextIn(dirFan[randDir]);
            }
        }
    }   
}