using GameLevel.Bricks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLevel
{
    [CreateAssetMenu(fileName = "SlotsFactory", menuName = "Arkanoid_Natoniewski/SlotsFactory")]
    public class SlotsFactory : ScriptableObject
    {
        public BrickSlot[] Slots;
        private Scene factoryScene;
        private string sceneName = "Factory";
        [SerializeField] private LevelData LevelData;
        [SerializeField] private Brick brickPrefab;

        public Brick SpawnBrickInto(BrickSlot slot)
        {
            CheckScene();
            
            Brick instance = Instantiate(brickPrefab) as Brick;
            slot.FillWith(instance);

            SceneManager.MoveGameObjectToScene(
                instance.gameObject, factoryScene
            );

            return instance;
        }
        private void SpawnSlots()
        {
            Slots = new BrickSlot[LevelData.LevelWidth * LevelData.LevelHeight];
            for (int i = 0, x = 0; x < LevelData.LevelWidth; x++)
            {
                for (int y = 0; y < LevelData.LevelHeight; y++)
                {
                    Slots[i++] = new BrickSlot(x, y);
                }
            }
            for (int i = 1; i < Slots.Length; i++)
            {
                Vector3 pos = Slots[i].transform.position;
                if (pos.x > 0)
                {
                    Slots[i].SetNeighbor(Direction.E, Slots[i - 1]);
                    if (pos.y > 0)
                    {
                        Slots[i].SetNeighbor(Direction.SE, Slots[i - LevelData.LevelWidth - 1]);
                        Slots[i].SetNeighbor(Direction.S, Slots[i - LevelData.LevelWidth]);
                    }
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
