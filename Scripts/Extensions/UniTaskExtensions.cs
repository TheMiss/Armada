using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Armageddon.Extensions
{
    public static class UniTaskExtensions
    {
        public static async UniTask WhenSucceeded<T>(this List<UniTask<T>> tasks)
        {
            while (true)
            {
                int succeededCount = tasks.Count(task => task.Status == UniTaskStatus.Succeeded);
                if (succeededCount == tasks.Count)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }
        
        public static async UniTask WhenSucceeded(this List<UniTask> tasks)
        {
            while (true)
            {
                int succeededCount = tasks.Count(task => task.Status == UniTaskStatus.Succeeded);
                if (succeededCount == tasks.Count)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }
    }
}
