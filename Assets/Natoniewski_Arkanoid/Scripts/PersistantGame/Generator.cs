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
        [Range(10, 100)] 
        public int RandomFillThreshold;
        public int RandomizedMaxThresholdOffset = 30;
        [Tooltip("The higher this number, the less frequent powerup drops are. Assumed to be at least 1 + max brick Hp")]
        public int PowerUpProbabilityLimiter;
        [Range(0f, 1f)]
        public float PowerUpSpawnThreshold;
        private bool symmetry;
        private int smoothTimes;
        private int seed = 123456789;
        private int randomRollCount;
        private System.Random pseudoRandom;
        private int[,] slots;
        private int[,] savedSlotsToRecreate;
        private List<Brick> activeBricks;
        private Factory<Brick> factory;
        [SerializeField] private Brick brickPrefab;
        [SerializeField] private LevelData LevelData;
        [SerializeField] private PowerUp[] PowerUps;


        private void Awake() => ResetSeed();
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
            writer.Write(symmetry ? 1 : 0);
            // state of random seed load
            writer.Write(randomRollCount);
            
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    writer.Write(savedSlotsToRecreate[x, y]);
                }
            }
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    writer.Write(slots[x, y]); // 0 (empty), 1,2,3, 4 (non destructible)
                }
            }
        }
        public override void Load(GameDataReader reader)
        {
            ResetChecks();
            // load the state of random seed
            randomRollCount = reader.ReadInt();
            for (int i = 0; i < randomRollCount; i++)
            {
                pseudoRandom.Next();
            }
            symmetry = reader.ReadInt() == 1;
            
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    savedSlotsToRecreate[x, y] = reader.ReadInt();
                }
            }
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    int hp = reader.ReadInt();
                    SetSlot(x, y, hp, symmetry);
                }
            }
        }
        public int RandomNext(int low, int high)
        {
            randomRollCount++;
            return pseudoRandom.Next(low, high);
        }
        public void ResetSeed()
        {
            randomRollCount = 0;
            pseudoRandom = new System.Random(seed);
        }
        private Brick GetBrick(
            Vector2 pos, int hp, int xIndex, int yIndex
        )
        {
            Brick instance = factory.Get();
            activeBricks.Add(instance);
            instance.SetUp(
                hp, LevelData.BrickColors,
                pos, xIndex, yIndex
            );
            return instance;
        }

        private void ReturnBrick(Brick b)
        {
            slots[b.XIndex, b.YIndex] = 0;
            b.PowerUpToSpawn = null;
            activeBricks.Remove(b);
            factory.Return(b);
            if (activeBricks.Count == 0)
            {
                NewLevel();
            }
        }
        private void ReturnBrickAt(int xIndex, int yIndex)
        {
            var brickToRemove = 
                activeBricks.Where(b => b.XIndex == xIndex && b.YIndex == yIndex);
            if (brickToRemove.Count() > 0)
            {
                Brick b = brickToRemove.First();
                ReturnBrick(b);
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
            if (savedSlotsToRecreate == null) 
            {
                savedSlotsToRecreate = 
                    new int[slots.GetLength(0), slots.GetLength(1)];
            }
            for (int i = 0; i < activeBricks.Count; i++)
            {
                factory.Return(activeBricks[i]);
            }
            activeBricks.Clear();
        }
        public void StartNewGame()
        {
            NewLevel();
        }
        public void NewLevel()
        {
            ResetChecks();
            GenerateMap();
        }
        /// <param name="x">Integer index</param>
        /// <param name="y">Integer index</param>
        /// <param name="hp">Brick hp</param>
        /// <param name="symm">Use symmetry about y axis?</param>
        private void SetSlot(int x, int y, int hp, bool symm = false)
        {
            if (slots[x, y] != 0) ReturnBrickAt(x, y);

            if (hp == 0)
            {
                ReturnBrickAt(x, y);
                return;
            }

            slots[x, y] = hp;
            Vector2 brickPos = symm && x > LevelData.LevelWidth / 2 ?
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

            Brick b = GetBrick(brickPos, hp, x, y);

            if (hp < 4)
            {
                float powerUpDropProbability = 1 / (float)(PowerUpProbabilityLimiter - hp);
                if (powerUpDropProbability >= PowerUpSpawnThreshold)
                {
                    b.PowerUpToSpawn = PowerUps[Random.Range(0, PowerUps.Length)];
                }
            }
        }
        public void RebuildGameLevel()
        {
            ResetChecks();
            for (int x = 0; x < LevelData.LevelWidth; x++)
            {
                for (int y = 0; y < LevelData.LevelHeight; y++)
                {
                    int brickHp = savedSlotsToRecreate[x, y];
                    SetSlot(x, y, brickHp, symmetry);
                }
            }
        }
        private void GenerateMap()
        {
            symmetry = RandomNext(0, 1) == 1;
            smoothTimes = RandomNext(0, 5);
            if (symmetry)
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
            for (int i = 0; i < smoothTimes; i++) 
            {
                SmoothMap();
            }

            if (symmetry)
                CopyRest();

            for (int x = 0; x < slots.GetLength(0); x++)
            {
                for (int y = 0; y < slots.GetLength(1); y++)
                {
                    savedSlotsToRecreate[x, y] = slots[x, y];
                }
            }
        }
        private void CopyRest()
        {
            for (int x = LevelData.LevelWidth / 2; x < LevelData.LevelWidth; x++) 
            {
                for (int y = 0; y < LevelData.LevelHeight; y++) 
                {
                    int hp = slots[x - LevelData.LevelWidth / 2, y];
                    SetSlot(
                        x, y,
                        hp, symmetry
                    );
                }
            }
        }
        private void RandomFillMap(int width) 
        {
            int thresholdModifier = RandomNext(
                -RandomizedMaxThresholdOffset, RandomizedMaxThresholdOffset
            );
            int resultingThreshold = System.Math.Clamp(
                RandomFillThreshold + thresholdModifier, 10, 100
            );
            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < LevelData.LevelHeight; y++) 
                {
                    int dice = RandomNext(0, 100);
                    if (dice <= resultingThreshold)
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
            int width = symmetry ? LevelData.LevelWidth / 2 : LevelData.LevelWidth;
            int height = symmetry ? LevelData.LevelWidth / 2 : LevelData.LevelWidth;
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