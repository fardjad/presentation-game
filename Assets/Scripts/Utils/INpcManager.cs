using Controllers;

namespace Utils
{
    public interface INpcManager
    {
        void RegisterNpcController(string id, NpcController npcController);
    }
}