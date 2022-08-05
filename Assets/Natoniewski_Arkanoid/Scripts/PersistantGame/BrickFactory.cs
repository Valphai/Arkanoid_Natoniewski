using GameLevel;
using GameLevel.Bricks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace PersistantGame
{
    [CreateAssetMenu(fileName = "BrickFactory", menuName = "Arkanoid_Natoniewski/BrickFactory")]
    public class BrickFactory : ScriptableObject
    {
        private Scene factoryScene;
        private string sceneName = "Factory";
        private ObjectPool<Brick> brickPool;
        [SerializeField] private Brick brickPrefab;

        public Brick GetBrick()
        {
            if (brickPool == null) 
            {
                CreatePool();
                CheckScene();
            }

            return brickPool.Get();
		}
        public void ReturnBrick(Brick brick)
        {
            if (brickPool == null) 
            {
                CreatePool();
                CheckScene();
            }
            brickPool.Release(brick);
        }
        private void CreatePool()
        {
            brickPool = new ObjectPool<Brick>(
                OnCreateItem,
                b => b.gameObject.SetActive(true), // onTake
                b => b.gameObject.SetActive(false), // onReturn
                b => Destroy(b.gameObject), // onDestroy
                collectionCheck : true // can not release released
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
