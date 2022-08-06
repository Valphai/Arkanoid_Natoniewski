using System.Collections.Generic;
using System.Linq;
using GameLevel;
using GameLevel.Bricks;
using GameLevel.PowerUps;
using GameSave;
using UnityEngine;

namespace PersistantGame
{
    public class Generator : IPersistantObject
    {
        public bool Symmetry;
        [Range(0,100)] 
        public int RandomFillThreshold;
        public int SmoothTimes;
        [Tooltip("The higher this number, the less frequent powerup drops are. Assumed to be at least 1 + maxHp")]
        public int PowerUpProbabilityLimiter;
        [Range(0f,1f)]
        public float PowerUpSpawnThreshold;
        private int seed;
        private System.Random pseudoRandom;
        private int[,] slots;
        private List<Brick> activeBricks;
        private Factory<Brick> factory;
        [SerializeField] private Brick brickPrefab;
        [SerializeField] private LevelData LevelData;
        [SerializeField] private PowerUp[] PowerUps;


        private void Awake()
        {
            seed = Random.Range(0, 100000);
            pseudoRandom = new System.Random(seed);
        }
        private void OnEnable()
        {
            Brick.OnBrickDestroyed += ReturnBrick;
            factory = new Factory<Brick>(
                brickPrefab, LevelData.FactoryName
            );
        }
        private void OnDisable() => Brick.OnBrickDestroyed -= ReturnBrick;
        public override void Save(GameDataWriter writer)
        {
            writer.Write(seed);
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    writer.Write(slots[x, y]); // 0 (empty), 1,2,3, 4 (non destructible)
                }
            }
        }
        private void ReturnBrick(Brick b)
        {
            factory.Return(b);
            activeBricks.Remove(b);
        }
        public override void Load(GameDataReader reader)
        {
            seed = reader.ReadInt();
            pseudoRandom = new System.Random(seed);
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    int hp = reader.ReadInt();
                    if (hp != 0) // not empty
                    {
                        Brick instance = factory.Get();
                        instance.SetUp(
                            hp, LevelData.BrickColors, 
                            x, y
                        );
                    }
                }
            }
        }
        public void StartNewGame()
        {
            if (slots == null) 
            {
                slots = 
                    new int[LevelData.LevelWidth, LevelData.LevelHeight];
            }
            if (activeBricks == null) activeBricks = new List<Brick>();

            for (int i = 0; i < activeBricks.Count; i++)
            {
                factory.Return(activeBricks[i]);
            }
            activeBricks.Clear();

            GenerateMap();
        }
        /// <param name="x">Integer index</param>
        /// <param name="y">Integer index</param>
        /// <param name="hp">Brick hp</param>
        /// <param name="symm">Use symmetry?</param>
        private void SetSlot(int x, int y, int hp, bool symm = false)
        {
            slots[x, y] = hp;
            Brick b;
            Vector2 brickPos = symm ? 
                new Vector2(
                    -((x - LevelData.LevelWidth / 2) * LevelData.BrickWidth + LevelData.XOffset), 
                    y * LevelData.BrickHeight + LevelData.YOffset
                )
                : 
                new Vector2(
                    x * LevelData.BrickWidth + LevelData.XOffset, 
                    y * LevelData.BrickHeight + LevelData.YOffset
                );
            if (hp == 0)
            {
                
                var brickToRemove = 
                    activeBricks.Where(b => (Vector2)b.transform.position == brickPos);
                if (brickToRemove.Count() > 0)
                {
                    b = brickToRemove.First();
                    activeBricks.Remove(b);
                    factory.Return(b);
                    b.PowerUpToSpawn = null;
                }
                return;
            }

            b = factory.Get();
            activeBricks.Add(b);
            b.SetUp(
                hp, LevelData.BrickColors, 
                brickPos
            );

            float powerUpDropProbability = 1 / (PowerUpProbabilityLimiter - hp);
            if (powerUpDropProbability >= PowerUpSpawnThreshold)
            {
                b.PowerUpToSpawn = PowerUps[Random.Range(0, PowerUps.Length)];
            }
        }
        private void GenerateMap()
        {
            if (Symmetry)
            {
                RandomFillMapSymmetry();
            }
            else
            {
                RandomFillMap();
    
                for (int i = 0; i < SmoothTimes; i++) 
                {
                    SmoothMap();
                }
            }
        }
        private void RandomFillMap() 
        {
            for (int x = 0; x < LevelData.LevelWidth; x++) 
            {
                for (int y = 0; y < LevelData.LevelHeight; y++) 
                {
                    float dice = pseudoRandom.Next(0,100);
                    if (dice <= RandomFillThreshold)
                    {
                        SetSlot(x, y, 1);
                    }
                    else
                    {
                        SetSlot(x, y, 0);
                    }
                }
            }
        }
        private void RandomFillMapSymmetry()
        {
            for (int x = 0; x < LevelData.LevelWidth / 2; x++) 
            {
                for (int y = 0; y < LevelData.LevelHeight; y++) 
                {
                    float dice = pseudoRandom.Next(0, 100);
                    if (dice <= RandomFillThreshold)
                    {
                        SetSlot(x, y, 1);
                        SetSlot(
                            x + LevelData.LevelWidth / 2, 
                            y, 1, Symmetry
                        );
                    }
                    else
                    {
                        SetSlot(x, y, 0);
                        SetSlot(
                            x + LevelData.LevelWidth / 2, 
                            y, 0, Symmetry
                        );
                    }
                }
            }
        }
        private void SmoothMap()
        {
            for (int x = 0; x < LevelData.LevelWidth; x++) 
            {
                for (int y = 0; y < LevelData.LevelHeight; y++) 
                {
                    int neighbourHpCount = GetSurroundingBrickHpCount(x, y);

                    if (neighbourHpCount < 2)
                        SetSlot(x, y, 1);

                    else if (neighbourHpCount > 2)
                        SetSlot(x, y, 0);
                }
            }
        }
        private int GetSurroundingBrickHpCount(int gridX, int gridY) 
        {
            int BrickHpCount = 0;
            for (int neighbX = gridX - 1; neighbX <= gridX + 1; neighbX++) 
            {
                for (int neighbY = gridY - 1; neighbY <= gridY + 1; neighbY++) 
                {
                    if (neighbX >= 0 && neighbX < LevelData.LevelWidth && neighbY >= 0 && neighbY < LevelData.LevelHeight) 
                    {
                        if (neighbX != gridX || neighbY != gridY) 
                        {
                            int hp = slots[neighbX, neighbY];
                            BrickHpCount += hp == 0 ? 0 : hp;
                        }
                    }
                    else 
                    {
                        BrickHpCount++;
                    }
                }
            }

            return BrickHpCount;
        }
    }   
}