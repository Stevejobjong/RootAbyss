using UnityEngine;
// Singleton Templete class 
// e.g. public class MyClassName : Singleton<MyClassName> {} 
// protected MyClassname() {} 을 선언해서 비 싱글톤 생성자 사용을 방지할 것 
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Destroy 여부 확인용 
    private static bool ShuttingDown = false;
    public static T Instance = null;
    static object _Lock = new object();
    public static T ins
    {
        get
        {
            // 게임 종료 시 Object 보다 싱글톤의 OnDestroy 가 먼저 실행 될 수도 있다. 
            // 해당 싱글톤을 gameObject.Ondestory() 에서는 사용하지 않거나 사용한다면 null 체크를 해주자 
            if (ShuttingDown)
            {
                Debug.Log("[Singleton] Instance '" + typeof(T) + "' already destroyed. Returning null.");
                return null;
            }

            lock (_Lock) //Thread Safe 
            {
                if (Instance == null)
                {
                    Instance = (T)FindObjectOfType(typeof(T));
                    if (Instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                    }
                }
                return Instance;
            }
        }

    }

    //private void OnApplicationQuit() { ShuttingDown = true; }
    //private void OnDestroy() { ShuttingDown = true; }
}