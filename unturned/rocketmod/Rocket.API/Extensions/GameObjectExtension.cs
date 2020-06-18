using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Extensions
{
    public static class GameObjectExtension
    {
        public static object TryAddComponent(this UnityEngine.GameObject gameobject, Type T)
        {
            return tryAddComponent(gameobject, T);
        }

        public static T TryAddComponent<T>(this UnityEngine.GameObject gameobject) where T : UnityEngine.Component
        {
            return (T)tryAddComponent(gameobject, typeof(T));
        }

        private static object tryAddComponent(UnityEngine.GameObject gameobject, Type T)
        {
            try
            {
                return gameobject.AddComponent(T);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("An error occured while adding component {0}", T.Name), ex);
            }
        }

        public static void TryRemoveComponent(this UnityEngine.GameObject gameobject,Type T)
        {
            tryRemoveComponent(gameobject, T);
        }

        public static void TryRemoveComponent<T>(this UnityEngine.GameObject gameobject) where T : UnityEngine.Component
        {
            TryRemoveComponent(gameobject,typeof(T));
        }

        private static void tryRemoveComponent(UnityEngine.GameObject gameobject, Type T)
        {
            try
            {
                UnityEngine.Object.Destroy(gameobject.GetComponent(T));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("An error occured while removing component {0}", T.Name), ex);
            }
        }


    }
}
