using GameLevel;
using GameLevel.Bricks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace PersistantGame
{
    [CreateAssetMenu(fileName = "SlotsFactory", menuName = "Arkanoid_Natoniewski/SlotsFactory")]
    public class SlotsFactory : ScriptableObject
    {
        private Scene factoryScene;
        private string sceneName = "Factory";
        private ObjectPool<Brick> brickPool;
        [SerializeField] private LevelData LevelData;
        [SerializeField] private Brick brickPrefab;

        public Brick PutBrickInto(BrickSlot slot) 
        {
            if (brickPool == null) 
            {
                CreatePool();
            }
            Brick instance = brickPool.Get();
            slot.FillWith(instance);

            return instance;
		}
        public void ReturnBrick(Brick brick) => brickPool.Release(brick);
        private void CreatePool()
        {
            brickPool = new ObjectPool<Brick>(
                OnCreateItem,
                b => b.gameObject.SetActive(true), // onTake
                b => b.gameObject.SetActive(false), // onReturn
                b => Destroy(b.gameObject), // onDestroy
                collectionCheck : true 
            );
        }
        private Brick OnCreateItem()
        {
            Brick instance = Instantiate(brickPrefab);
            SceneManager.MoveGameObjectToScene(
                instance.gameObject, factoryScene
            );
            return instance;
        }

        public void SpawnSlots(ref BrickSlot[] slots)
        {
            slots = new BrickSlot[LevelData.LevelWidth * LevelData.LevelHeight];
            CheckScene();

            for (int i = 0, y = 0; y < LevelData.LevelHeight; y++)
            {
                for (int x = 0; x < LevelData.LevelWidth; x++)
                {
                    slots[i++] = new BrickSlot(
                        x * LevelData.BrickWidth + LevelData.XOffset,
                        y * LevelData.BrickHeight + LevelData.YOffset
                    );
                }
            }
            for (int i = 1; i < slots.Length; i++)
            {
                Vector3 pos = slots[i].Position();
                if (pos.x > LevelData.XOffset)
                {
                    slots[i].SetNeighbor(Direction.E, slots[i - 1]);
                    if (pos.y > LevelData.YOffset)
                    {
                        slots[i].SetNeighbor(Direction.SE, slots[i - LevelData.LevelWidth - 1]);
                    }
                    if (pos.y < LevelData.LevelHeight * LevelData.BrickHeight + LevelData.YOffset)
                    {
                        slots[i].SetNeighbor(Direction.NE, slots[i + LevelData.LevelWidth - 1]);
                    }
                }
                if (pos.y > LevelData.YOffset)
                {
                    slots[i].SetNeighbor(Direction.S, slots[i - LevelData.LevelWidth]);
                }
            }
        }
        private void CheckScene()
        {
            if (Application.isEditor)
            {
                factoryScene = SceneManager.GetSceneByName(sceneName);
                if (factoryScene.isLoaded) 
                    return;
            }
            
            factoryScene = SceneManager.CreateScene(sceneName);
        }
    }
    
}
