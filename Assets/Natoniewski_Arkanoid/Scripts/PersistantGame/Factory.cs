using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace PersistantGame
{
    public class Factory<T> where T : MonoBehaviour
    {
        private ObjectPool<T> pool;
        private Scene factoryScene;
        private readonly string sceneName;
        private readonly T prefab;

        public Factory(T prefab, string sceneName)
        {
            this.prefab = prefab;
            this.sceneName = sceneName;
        }
        public T Get()
        {
            if (pool == null) 
            {
                CreatePool();
                CheckScene();
            }

            return pool.Get();
		}
        public void Return(T obj)
        {
            if (pool == null) 
            {
                CreatePool();
                CheckScene();
            }
            pool.Release(obj);
        }
        private void CreatePool()
        {
            pool = new ObjectPool<T>(
                OnCreateItem,
                b => b.gameObject.SetActive(true), // onTake
                b => b.gameObject.SetActive(false), // onReturn
                b => MonoBehaviour.Destroy(b.gameObject), // onDestroy
                collectionCheck : true // can not release released
            );
        }
        private T OnCreateItem()
        {
            T instance = MonoBehaviour.Instantiate(prefab);
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
