using UnityEngine;

// Courtesy of: http://wiki.unity3d.com/index.php/Toolbox

public class Toolbox : Singleton<Toolbox>
{
    protected Toolbox()
    {
    }

    private void Awake()
    {
        // TODO: Register Items
    }

    private static T RegisterComponent<T>() where T : Component
    {
        return Instance.GetOrAddComponent<T>();
    }
}