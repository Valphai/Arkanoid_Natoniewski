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
        [Tooltip("The higher this number, the less frequent powerup drops are. Assumed to be at least 1 + max brick Hp")]
        public int PowerUpProbabilityLimiter;
        [Range(0f,1f)]
        public float PowerUpSpawnThreshold;
        private int seed;
        private System.Random pseudoRandom;
        private int[,] slots;
        private int[,] savedSlotsToRecreate;
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
        private void ResetChecks()
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
        }
        public void StartNewGame()
        {
            ResetChecks();
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
                /* 
                    if using symmetry we need x index to be offset by LevelWidth / 2,
                    hence here we get rid of this offset when setting position
                */
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
        public void RebuildGameLevel()
        {
            ResetChecks();
            if (Symmetry)
            {
                for (int x = 0; x < LevelData.LevelWidth / 2; x++)
                {
                    for (int y = 0; y < LevelData.LevelHeight; y++)
                    {
                        int brickHp = savedSlotsToRecreate[x, y];
                        SetSlot(x, y, brickHp);
                    }
                }
                for (int x = 0; x < LevelData.LevelWidth / 2; x++)
                {
                    for (int y = 0; y < LevelData.LevelHeight; y++)
                    {
                        int brickHp = savedSlotsToRecreate[x, y];
                        SetSlot(x + LevelData.LevelWidth / 2, y, brickHp, 
                            Symmetry
                        );
                    }
                }
                return;
            }
            for (int x = 0; x < LevelData.LevelWidth; x++)
            {
                for (int y = 0; y < LevelData.LevelHeight; y++)
                {
                    int brickHp = savedSlotsToRecreate[x, y];
                    SetSlot(x, y, brickHp);
                }
            }
        }
        private void GenerateMap()
        {
            if (Symmetry)
            {
                RandomFillMap(
                    LevelData.LevelWidth / 2
                );
            }
            else
            {
                RandomFillMap(
                    LevelData.LevelWidth
                );
    
            }
            for (int i = 0; i < SmoothTimes; i++) 
            {
                SmoothMap();
            }

            if (Symmetry)
                CopyRest();

            savedSlotsToRecreate = slots;
        }

        private void CopyRest()
        {
            for (int x = 0; x < LevelData.LevelWidth / 2; x++) 
            {
                for (int y = 0; y < LevelData.LevelHeight; y++) 
                {
                    SetSlot(
                        x + LevelData.LevelWidth / 2, 
                        y, slots[x, y], Symmetry
                    );
                }
            }
        }

        private void RandomFillMap(
            int width
        ) 
        {
            for (int x = 0; x < width; x++) 
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
        private void SmoothMap()
        {
            int width = Symmetry ? LevelData.LevelWidth / 2 : LevelData.LevelWidth;
            int height = Symmetry ? LevelData.LevelWidth / 2 : LevelData.LevelWidth;
            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    int neighbourHpCount = GetSurroundingBrickHpCount(x, y);

                    if (neighbourHpCount < 4)
                        SetSlot(x, y, 1);
                    else if (neighbourHpCount == 5)
                        SetSlot(x, y, 3);
                    else if (neighbourHpCount == 6)
                        SetSlot(x, y, 4);
                    else if (neighbourHpCount <= 8) // 7, 8
                        SetSlot(x, y, 2);
                    else if (neighbourHpCount >= 2)
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