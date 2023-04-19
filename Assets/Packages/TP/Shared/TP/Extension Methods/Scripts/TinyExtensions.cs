using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TP.ExtensionMethods
{
    /// <summary>
    /// Usefull extenstions methods intened to increase productivity and readablity.
    /// </summary>
    public static class ExtensionMethods
    {
        public static bool masterVerbose = false;

        /// <summary>
        /// Selects an element from a list at random. Each
        /// Item has a weight value and higher weights
        /// increase their chance of being chosen.
        /// </summary>
        /// <param name="weights">A list of weights in an Item list</param>
        /// <returns>The index of the Item in the original list to be picked</returns>
        public static int GetWeightedIndex(this List<float> weights)
        {
            int index = -1;
            float maxChoice = 0;
            foreach (float weight in weights)
            {
                maxChoice += weight;
            }
            float randChoice = Random.Range(0, maxChoice);
            float weightSum = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                weightSum += weights[i];
                if (randChoice <= weightSum)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static Bounds Encapsulate(this Bounds boundsA, Bounds boundsB, bool ignoreEmpty)
        {
            if (!ignoreEmpty)
            {
                boundsA.Encapsulate(boundsB);
            }
            else
            {
                if (boundsB.extents != Vector3.zero)
                {
                    if (boundsA.extents == Vector3.zero)
                    {
                        boundsA = boundsB;
                    }
                    else
                    {
                        boundsA.Encapsulate(boundsB);
                    }
                }
            }

            return boundsA;
        }

        public static bool IsPrefab(this GameObject gameObject)
        {
            bool isPrefab = false;

            if (gameObject != null && gameObject.scene.name == null ||
                gameObject != null && gameObject.gameObject != null && gameObject.gameObject.scene.name == null)
            {
                isPrefab = true;
            }

            return isPrefab;
        }

        /// <summary>
        /// Destroys every GameObject within a list.
        /// </summary>
        /// <param name="gameObjectList">The list of GameObjects to be destroyed</param>
        public static void BlowUp(this List<GameObject> gameObjectList)
        {
            for (int i = gameObjectList.Count - 1; i >= 0; i--)
            {
                gameObjectList[i].BlowUp();
            }
            gameObjectList.Clear();
        }

        /// <summary>
        /// Overload for BlowUp to blow self up with no delay.
        /// </summary>
        /// <param name="objectToBlowUp">A Self GameObject reference</param>
        /// <param name="destroyInEditMode">Destroy the GO even if this happens in edit mode.</param>
        public static void BlowUp(this Object objectToBlowUp, bool destroyInEditMode = true)
        {
            BlowUp(objectToBlowUp, 0, destroyInEditMode);
        }

        /// <summary>
        /// Overload for BlowUpDelayed to blow self up with a specified delay.
        /// </summary>
        /// <param name="gameObject">A Self GameObject reference</param>
        /// <param name="delay">How long to delay the BlowUp</param>
        /// <param name="destroyInEditMode">Destroy the GO even if this happens in edit mode.</param>
        public static void BlowUp(this Object objectToBlowUp, float delay, bool destroyInEditMode = true)
        {
            BlowUpDelayed(objectToBlowUp, delay, destroyInEditMode);
        }

        /// <summary>
        /// Destroys the provided GameObject. Has a delay if the application
        /// is playing, otherwise it is destroyed immediately.

        /// The reason for this is that destroy doesnt work in editor mode. 
        /// This allows us to use the same destroy logic whether the game is
        /// playing or not.
        /// </summary>
        /// <param name="gameObject">The GameObject to destroy</param>
        /// <param name="delay">How long to delay if the application is playing</param>
        private static void BlowUpDelayed(Object objectToBlowUp, float delay, bool destroyInEditMode = true)
        {
            try // There is a chance the GameObject has already been destroyed
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(objectToBlowUp, delay);
                }
                else if (destroyInEditMode)
                {
                    GameObject.DestroyImmediate(objectToBlowUp);
                }
            }
            catch (NullReferenceException exception)
            {
                // Print debug message and include the exception message in order to remove unused variable warning
                Debug.Log("Tried destroying " + objectToBlowUp + " but value is null: " + exception.Message);
            }
        }

        /// <summary>
        /// Gets the closes active GameObject to the origin GameObject.
        /// </summary>
        /// <param name="origin">A Self GameObject reference</param>
        /// <param name="gameObjectList">An IEnumerable List of gameObjects to pick the closest one from</param>
        /// <returns>A reference to the nearest GameObject from the provided list</returns>
        public static T NearestComponent<T>(this Vector3 origin, IEnumerable<T> gameObjectList) where T : Component
        {
            T nearest = null;
            Vector3 nearestPositionOffset = Vector3.zero;

            foreach (T gameObject in gameObjectList)
            {
                GameObject tempGameObject = (gameObject as Component).gameObject;

                if (tempGameObject.activeInHierarchy)
                {
                    Vector3 tempPositionOffset = origin - tempGameObject.transform.position;
                    if (nearestPositionOffset == Vector3.zero || nearestPositionOffset.LongerThan(tempPositionOffset))
                    {
                        nearestPositionOffset = tempPositionOffset;
                        nearest = gameObject;
                    }
                }
            }

            return nearest;
        }

        /// <summary>
        /// Gets the closes active GameObject to the origin GameObject.
        /// </summary>
        /// <param name="origin">A Self GameObject reference</param>
        /// <param name="gameObjectList">An IEnumerable List of gameObjects to pick the closest one from</param>
        /// <returns>A reference to the nearest GameObject from the provided list</returns>
        public static GameObject Nearest<T>(this Vector3 origin, IEnumerable<T> gameObjectList) where T : Component
        {
            GameObject nearest = null;
            Vector3 nearestPositionOffset = Vector3.zero;

            foreach (T gameObject in gameObjectList)
            {
                GameObject tempGameObject = (gameObject as Component).gameObject;

                if (tempGameObject.activeInHierarchy)
                {
                    Vector3 tempPositionOffset = origin - tempGameObject.transform.position;
                    if (nearestPositionOffset == Vector3.zero || nearestPositionOffset.LongerThan(tempPositionOffset))
                    {
                        nearestPositionOffset = tempPositionOffset;
                        nearest = tempGameObject;
                    }
                }
            }

            return nearest;
        }

        /// <summary>
        /// Gets the closes active GameObject to the origin GameObject.
        /// </summary>
        /// <param name="origin">A Self GameObject reference</param>
        /// <param name="gameObjectList">An IEnumerable List of gameObjects to pick the closest one from</param>
        /// <returns>A reference to the nearest GameObject from the provided list</returns>
        public static GameObject Nearest<T>(this GameObject origin, IEnumerable<T> gameObjectList) where T : Component
        {
            return origin.transform.position.Nearest<T>(gameObjectList);
        }

        /// <summary>
        /// Returns all the GameObjects in a radius of range. Optionally returns only the first x (limit) found in that
        /// range.
        /// </summary>
        /// <param name="origin">A Self GameObject reference</param>
        /// <param name="gameObjectList">An IEnumerable List of gameObjects to pick the closest one from</param>
        /// <returns>A reference to the nearest GameObject from the provided list</returns>
        public static List<GameObject> InRange<T>(this GameObject origin, IEnumerable<T> gameObjectList, float range, float limit = -1) where T : Component
        {
            List<GameObject> inRange = new List<GameObject>();
            Vector3 nearestPositionOffset = Vector3.zero;

            foreach (T gameObject in gameObjectList)
            {
                GameObject tempGameObject = (gameObject as Component).gameObject;

                if (tempGameObject.activeInHierarchy)
                {
                    Vector3 tempPositionOffset = origin.transform.position - tempGameObject.transform.position;
                    if (nearestPositionOffset == Vector3.zero || nearestPositionOffset.ShorterThan(range))
                    {
                        nearestPositionOffset = tempPositionOffset;
                        inRange.Add(tempGameObject);

                        if (inRange.Count == limit)
                        {
                            break;
                        }
                    }
                }
            }

            return inRange;
        }

        /// <summary>
        /// Removes an element at a given index from an array in place.
        /// </summary>
        /// <typeparam name="T">The type of array we are working with</typeparam>
        /// <param name="arr">The array to have an element removed</param>
        /// <param name="index">The index of the element to remove</param>
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }

        /// <summary>
        /// Add one new element to the end of an array.
        /// </summary>
        /// <typeparam name="T">The type of array we are working with</typeparam>
        /// <param name="array">The array to be appened; self referential</param>
        /// <param name="itemToAppend">The element to be added to the end of the array</param>
        /// <returns></returns>
        public static T[] Prepend<T>(this Array array, T itemToPrepend)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, 0, newArray, 1, array.Length);
            newArray[0] = itemToPrepend;
            return newArray;
        }

        /// <summary>
        /// Add one new element to the end of an array.
        /// </summary>
        /// <typeparam name="T">The type of array we are working with</typeparam>
        /// <param name="array">The array to be appened; self referential</param>
        /// <param name="itemToAppend">The element to be added to the end of the array</param>
        /// <returns></returns>
        //public static T[] Append<T>(this Array array, T itemToAppend)
        //{
        //    T[] newArray = new T[array.Length + 1];
        //    Array.Copy(array, newArray, array.Length);
        //    newArray[newArray.Length - 1] = itemToAppend;
        //    return newArray;
        //}

        /// <summary>
        /// Removes the last element from an array
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="array">The array to have the element removed; self referential</param>
        /// <returns></returns>
        public static T[] RemoveLast<T>(this Array array)
        {
            T[] newArray = new T[array.Length - 1];
            Array.Copy(array, newArray, newArray.Length);
            return newArray;
        }

        /// <summary>
        /// An array overload for GetComponentInChildrenAndSelf
        /// </summary>
        /// <typeparam name="T">The component type to be found</typeparam>
        /// <param name="gameObject">The GaneObject to get these components from</param>
        /// <returns>An array of the components of type T found on the given GameObject or its children</returns>
        public static T[] GetComponentsInChildrenAndSelf<T>(this GameObject gameObject)
        {
            return gameObject.GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Convert a 2D point to 3D. INCOMPLETE
        /// </summary>
        /// <param name="vector2">The point to be converted; self referential</param>
        /// <returns>The 3D point</returns>
        public static Vector3 ToVector3(this Vector2 vector2)
        {
            //  TODO: Update this to use the current up direction for the game rather than
            //  assuming that z is up. or take a second param "axis" of type int.
            return new Vector3(vector2.x, vector2.y, 0);
        }

        public static float Polartiy(this float number)
        {
            return number != 0 ? Mathf.Abs(number) / number : 0;
        }

        public static float AngleChange(this float angleA, float angleB)
        {
            float angleChange = angleA.WrapBetween(0, 360) - angleB.WrapBetween(0, 360);
            if (Mathf.Abs(angleChange) > 180)
            {
                angleChange = -angleChange.Polartiy() * 360 + angleChange;
            }

            return angleChange;
        }

        public static Vector3 ToVector3(this float number)
        {
            return new Vector3(number, number, number);
        }

        public static Vector2 ToVector2(this float number)
        {
            return new Vector2(number, number);
        }

        public static int ParentDepth(this Transform transform)
        {
            int parentCount = 0;

            Transform root = transform.root;
            Transform current = transform;

            int maxParents = transform.hierarchyCount;

            for (; current != root; parentCount++)
            {
                current = current.parent;

                if (parentCount > maxParents)
                {
                    Debug.Log(@"TinyExtensions.ParentDepth: Something went wrong while attepting to calculate parent depth.
                        Root transform was not detected properly.");

                    break;
                }
            }

            return parentCount;
        }

        public static void AddUnique<T>(this List<T> list, T newItem)
        {
            if (newItem != null && !list.Contains<T>(newItem))
            {
                list.Add(newItem);
            }
        }

        public static void AddUniqueRange<T>(this List<T> list, List<T> range)
        {
            foreach (T item in range)
            {
                if (!list.Contains<T>(item))
                {
                    list.Add(item);
                }
            }
        }

        public static void RemoveRange<T>(this List<T> list, List<T> range)
        {
            foreach (T item in range)
            {
                list.Remove(item);
            }
        }
        
        //https://thomaslevesque.com/2019/11/18/using-foreach-with-index-in-c/
        // Example:
        // foreach (var (item, index) in collection.WithIndex())
        // {
        //     DoSomething(item, index);
        // }
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        public static List<Type> RequiredComponents (this Component component)
        {
            List<Type> requiredComponentTypes = new List<Type> { };

            Type componentType = component.GetType();
            RequireComponent[] requireComponentAttrs = Attribute.GetCustomAttributes(componentType, typeof(RequireComponent), true) as RequireComponent[];

            foreach (RequireComponent requireComponentAttr in requireComponentAttrs)
            {                    
                requiredComponentTypes.AddUnique(requireComponentAttr.m_Type0);
                requiredComponentTypes.AddUnique(requireComponentAttr.m_Type1);
                requiredComponentTypes.AddUnique(requireComponentAttr.m_Type2);
            }

            return requiredComponentTypes;
        }

        public static List<Component> RequiredByComponents (this Component component, List<Component> componentsToCheck)
        {
            List<Component> requiredByComponents = new List<Component> { };
            
            Type componentType = component.GetType();

            foreach (Component componentToCheck in componentsToCheck)
            {
                List<Type> requiredComponents = RequiredComponents(componentToCheck);

                if (requiredComponents.Contains(componentType))
                {
                    requiredByComponents.Add(componentToCheck);
                }
            }

            return requiredByComponents;
        }

        public static bool IsOnNavMesh(this Vector3 position)
        {
            NavMeshHit hit = new NavMeshHit();
            return NavMesh.SamplePosition(position, out hit, 1, NavMesh.AllAreas);
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool LongerThan(this Vector3 vectorA, float lengthB)
        {
            bool isLongerThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = lengthB * lengthB;
            if (squareLengthA > squareLengthB)
            {
                isLongerThan = true;
            }

            return isLongerThan;
        }
        
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="lengthB"></param>
        /// <returns>True if vectorA is longer than lengthB</returns>
        public static bool ShorterThan(this Vector3 vectorA, float lengthB)
        {
            bool isShorterThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = lengthB * lengthB;
            if (squareLengthA < squareLengthB)
            {
                isShorterThan = true;
            }

            return isShorterThan;
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool LongerThan(this Vector3 vectorA, Vector3 vectorB)
        {
            bool isLongerThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = vectorB.sqrMagnitude;
            if (squareLengthA > squareLengthB)
            {
                isLongerThan = true;
            }

            return isLongerThan;
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static bool ShorterThan(this Vector3 vectorA, Vector3 vectorB)
        {
            bool isShorterThan = false;

            float squareLengthA = vectorA.sqrMagnitude;
            float squareLengthB = vectorB.sqrMagnitude;
            if (squareLengthA < squareLengthB)
            {
                isShorterThan = true;
            }

            return isShorterThan;
        }
        
        /// <summary>
        /// Quick way to comparing distances. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="distance"></param>
        /// <returns>True if the difference between vectorA and VectorB is within the distance.</returns>
        public static bool WithinDistance(this Vector3 vectorA, Vector3 vectorB, float distance)
        {
            bool isWithinDistance = (vectorA - vectorB).ShorterThan(lengthB:distance);

            return isWithinDistance;
        }
        
        
        
        /// <summary>
        /// Accurate way of move towards something and snapping exactly to the final value. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static Vector3 MoveTowardsSnap(this Vector3 vectorA, Vector3 vectorB, float maxDistanceDelta)
        {
            if ((vectorA - vectorB).ShorterThan(maxDistanceDelta))
            {
                vectorA = vectorB;
            }
            else
            {
                vectorA = Vector3.MoveTowards(vectorA, vectorB, maxDistanceDelta);
            }

            return vectorA;
        }

        /// <summary>
        /// Convert a 3D point to 2D. INCOMPLETE
        /// </summary>
        /// <param name="vector3">The point to be converted; self referential</param>
        /// <returns>The 2D point</returns>
        public static Vector2 ToVector2(this Vector3 vector3)
        {
            //  TODO: Update this to use the current up direction for the game rather than
            //  assuming that z is up. or take a second param "axis" of type int.
            return new Vector2(vector3.x, vector3.z);
        }
        
        /// <summary>
        /// Accurate way of rotate towards something and snapping exactly to the final value. 
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <returns>True if vectorA is longer</returns>
        public static Quaternion RotateTowardsSnap(this Quaternion rotationA, Quaternion rotationB, float maxDegreesDelta)
        {
            if (Quaternion.Angle(rotationA, rotationB) < maxDegreesDelta)
            {
                rotationA = rotationB;
            }
            else
            {
                rotationA = Quaternion.RotateTowards(rotationA, rotationB, maxDegreesDelta);
            }

            return rotationA;
        }

        public static Vector3 SmoothRotation(this Vector3 newRotation, Vector3 previousRotation)
        {
            newRotation.x = newRotation.x.SmoothRotation(previousRotation.x);
            newRotation.y = newRotation.y.SmoothRotation(previousRotation.y);
            newRotation.z = newRotation.z.SmoothRotation(previousRotation.z);

            return newRotation;
        }

        public static float SmoothRotation(this float newRotation, float previousRotation)
        {
            float rotationChange = newRotation - previousRotation;

            if (Mathf.Abs(rotationChange) > 180)
            {
                float parity = rotationChange / Mathf.Abs(rotationChange);
                float fullRotations = Mathf.Round(rotationChange / 360);
                newRotation += 360 * fullRotations * -parity;
                rotationChange = newRotation - previousRotation;
            }

            return newRotation;
        }
        
        public static float WrapBetween(this float value, float limitA, float limitB)
        {
            float adjustedValue;

            float min = Mathf.Min(limitA, limitB);
            float max = Mathf.Max(limitA, limitB);

            adjustedValue = Mathf.Repeat(value - min, max - min) + min;

            return adjustedValue;
        }
                
        /// <summary>
        /// Check to see if a layermask contains an individual layer.
        /// </summary>
        /// <param name="vector3">The point to be converted; self referential</param>
        /// <returns>The 2D point</returns>
        public static bool Contains(this LayerMask layerMask, int layer)
         {
             return layerMask == (layerMask | (1 << layer));
         }

        /// <summary>
        /// Scales a rect by a given factor
        /// </summary>
        /// <param name="rect">The rect to scale; self referential</param>
        /// <param name="scale">The factor to scale by</param>
        /// <returns>The scaled rect</returns>
        public static Rect ScaledCopy(this Rect rect, float scale)
        {
            float widthChange = rect.width * scale - rect.width;
            float heightChange = rect.height * scale - rect.height;

            rect.position = new Vector2(
                rect.x - widthChange / 2,
                rect.y - heightChange / 2);

            rect.width += widthChange;
            rect.height += heightChange;

            return rect;
        }

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            // TODO: rectTransform.sizeDelta.x / 2 only works when the rect anchor is centered. Need to update to support other anchor types.
            return new Rect(
                rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x / 2,
                rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y / 2,
//                rectTransform.anchoredPosition3D.z,
                rectTransform.sizeDelta.x,
                rectTransform.sizeDelta.y
            );
        }

        #region Enumerable Management

        /// <summary>
        /// Picks an element at random from an array
        /// </summary>
        /// <typeparam name="T">The array type</typeparam>
        /// <param name="array">The array to pick from; self referential</param>
        /// <returns>The randomly selected element</returns>
        public static T PickRandom<T>(this Array array)
        {
            int randIndex = UnityEngine.Random.Range(0, array.Length);
            T randomPick = (T)array.GetValue(randIndex);

            return randomPick;
        }

        /// <summary>
        /// Picks an element at random from an IEnumerable list.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type</typeparam>
        /// <param name="list">The list to pick an element from; self referential</param>
        /// <returns>The randomly selected element</returns>
        public static T PickRandom<T>(this IEnumerable<T> list, IEnumerable<T> exclude = null)
        {
            T randomPick;
            if (exclude != null)
            {
                randomPick = list.PickRandom(1, exclude).First();
            }
            else
            {
                int randIndex = UnityEngine.Random.Range(0, list.Count());
                randomPick = list.ElementAt(randIndex);
            }
            
            return randomPick;
        }

        /// <summary>
        /// Picks a random number of elements at random from within an IEnumerable list.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type</typeparam>
        /// <param name="list">The list to pick from; self referential</param>
        /// <param name="minNuberToPick">The min number of items to pick [inclusive]</param>
        /// <param name="maxNuberToPick">The max number of items to pick [exclusive]</param>
        /// <returns>The randomly chosen elements</returns>
        public static List<T> PickRandom<T>(this IEnumerable<T> list, int minNuberToPick, int maxNuberToPick, IEnumerable<T> exclude = null)
        {
            return PickRandom<T>(list, Random.Range(minNuberToPick, maxNuberToPick), exclude);
        }

        /// <summary>
        /// Picks a given number of random elements from an IEnumerable list.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type</typeparam>
        /// <param name="list">The list to pick from; self referential</param>
        /// <param name="numberToPick">The number of elements that will be chosen at random</param>
        /// <returns>The randomly chosen elements</returns>
        public static List<T> PickRandom<T>(this IEnumerable<T> list, int numberToPick, IEnumerable<T> exclude = null)
        {
            if (numberToPick > list.Count())
            {
                // This does not cause any error other than the list being shorter than expected,
                // but still should not occur so it is logged for debugging
                Debug.LogWarning("PickRandom: Attempted to pick more elements than exist in list");
            }

            List<T> randomPicks = new List<T> { };
            List<T> notPicked = new List<T>(list);
            if (exclude != null)
            {
                notPicked.RemoveAll(exclude.Contains);
            }

            for (int i = 0; i < numberToPick && notPicked.Count > 0; i++)
            {
                int randIndex = Random.Range(0, notPicked.Count);
                T randT = notPicked[randIndex];
                randomPicks.Add(randT);
                notPicked.Remove(randT);
            }

            return randomPicks;
        }

        #endregion

        #region Deep Equality Checking

        public delegate bool ValueEqualsFunc<T>(T itemA, T itemB);
                
        public static bool ValueEquals(this GameObject gameObjectA, GameObject gameObjectB)
        {
            return DeepEqualsGameObj(gameObjectA, gameObjectB);
        }
        
        public static bool ValueEquals(this object objectA, object objectB)
        {
            return DeepEquals(objectA, objectB);
        }

        private static bool DeepEqualsGameObj(GameObject gameObjectA, GameObject gameObjectB)
        {
            bool childrenMatch = gameObjectA.Children().ValueScrambledEquals(gameObjectB.Children(), DeepEqualsGameObj);
            bool componentsMatch = gameObjectA.GetComponents<Component>().ValueScrambledEquals(gameObjectB.GetComponents<Component>(), DeepEquals);

            return childrenMatch && componentsMatch;
        }

        private static bool DeepEquals(object objectA, object objectB)
        {
            bool equal = true;

            if (objectA != null && objectB != null)
            {
                Type type = objectA.GetType();

                if (objectA.GetType() != objectB.GetType())
                {
                    equal = false;
                }
                else if (type != typeof(Transform))
                {
                    equal = false;
                    
                    if (objectA.Equals(objectB))
                    {
                        equal = true;
                    }
                    else if (type.IsSubclassOf(typeof(Component)))
                    {   
                        FieldInfo[] fields = objectA.GetType().GetFieldsComparable().ToArray();
                        PropertyInfo[] properties = objectA.GetType().GetPropertiesComparable().ToArray();

                        bool verbose = false;
                        string[] fieldNames = null;
                        string[] propertyNames = null;
                        if (verbose || masterVerbose)
                        {
                            fieldNames = Array.ConvertAll(fields, fieldInfo => fieldInfo.Name);
                            propertyNames = Array.ConvertAll(properties, fieldInfo => fieldInfo.Name);
                        }

                        object[] fieldsValueA = Array.ConvertAll(fields, fieldInfo => fieldInfo.GetValueNoError(objectA, true));
                        object[] fieldsValueB = Array.ConvertAll(fields, fieldInfo => fieldInfo.GetValueNoError(objectB, true));
                        bool fieldsEqual = fieldsValueA.ValueOrderedEquals(fieldsValueB, DeepEquals, verbose, fieldNames);
                        
                        object[] propertiesValueA = Array.ConvertAll(properties, propertyInfo => propertyInfo.GetValueNoError(objectA));
                        object[] propertiesValueB = Array.ConvertAll(properties, propertyInfo => propertyInfo.GetValueNoError(objectB));
                        bool propertiesEqual = propertiesValueA.ValueOrderedEquals(propertiesValueB, DeepEquals, verbose, propertyNames);

                        equal = fieldsEqual && propertiesEqual;
                    }
                }
            }
            
            return equal;
        }
        
        public static bool ValueOrderedEquals<T>(this T[] setA, T[] setB, ValueEqualsFunc<T> valueEquals, bool verbose=false, string[] names=null)
        {
            bool equals = true;
            int length = setA.Length;

            if (setA.Length != setB.Length)
            {
                equals = false;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    T itemA = setA[i];
                    T itemB = setB[i];

                    if (!valueEquals(itemA, itemB))
                    {
                        equals = false;

                        if (verbose || masterVerbose)
                        {
                            string itemNameA = names[i];
                            string itemValueA = itemA != null ? "null" : itemA.ToString();
                            string itemNameB = names[i];
                            string itemValueB = itemB != null ? "null" : itemB.ToString();

                            Debug.Log(String.Concat("Non-Match: (", itemNameA, ") ", itemA.ToString(), " != ", itemB.ToString()));
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //if ((verbose || masterVerbose) && itemA != null)
                        //{
                        //    string itemNameA = names[i];
                        //    string itemValueA = itemA != null ? "null" : itemA.ToString();

                        //    Debug.Log(String.Concat("Match: (", itemNameA, ") ", itemValueA));
                        //}
                    }
                }
            }

            return equals;
        }
        
        public static bool ValueScrambledEquals<T>(this T[] setA, T[] setB, ValueEqualsFunc<T> valueEquals, bool verbose=false, string[] names=null) 
        {
            bool equals = true;
            int lengthA = setA.Length;
            int lengthB = setB.Length;

            if (lengthA != lengthB)
            {
                equals = false;
            }
            else
            { 
                bool[] indexMatchedA = new bool[lengthA];
                bool[] indexMatchedB = new bool[lengthB];

                for (int ia = 0; ia < lengthA; ia++)
                {
                    if (indexMatchedA[ia]) { continue; }

                    bool matchFound = false;

                    T itemA = setA[ia];
                    for (int ib = 0; ib < lengthB; ib++)
                    {
                        if (indexMatchedB[ib]) { continue; }

                        T itemB = setB[ib];
                        if (valueEquals(itemA, itemB))
                        {
                            indexMatchedA[ia] = true;
                            indexMatchedB[ib] = true;
                            matchFound = true;
                            break;
                        }
                    }

                    if (!matchFound)
                    {
                        equals = false;

                        break;
                    }
                }
            }

            return equals;
        }

        #endregion

        public static GameObject[] Children(this GameObject gameObject)
        {
            Transform transform  = gameObject.transform;
            int childCount = transform.childCount;
            GameObject[] children = new GameObject[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i).gameObject;
            }

            return children;
        }

        public static void IgnoreCollision(this Rigidbody rigidbodyA, Rigidbody rigidbodyB, bool ignore = true)
        {
            foreach(Collider colliderA in rigidbodyA.gameObject.GetComponentsInChildren<Collider>())
            {
                foreach(Collider colliderB in rigidbodyB.gameObject.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(colliderA, colliderB, ignore);
                }
            }
        }

        public static void IgnoreCollision(this Rigidbody rigidbodyA, Collider colliderB, bool ignore = true)
        {
            foreach(Collider colliderA in rigidbodyA.gameObject.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(colliderA, colliderB, ignore);
            }
        }

        public static bool includeNames = true;

        public static List<string> CantCopy(this Type type)
        {
            List<string> types = new List<string> { };

            if (type.IsSubclassOf(typeof(Component)))
            {
                //if (!includeNames)
                //{
                //types.Add(string.Concat(type, ".name"));
                //}
                types.Add(string.Concat(type, ".mesh"));
                types.Add(string.Concat(type, ".material"));
                types.Add(string.Concat(type, ".materials"));   
            }

            return types;
        }

        public static List<string> CantCompare(this Type type)
        {
            List<string> types = new List<string> { };
            
            if (type.IsSubclassOf(typeof(Component)))
            {
                ///if (!includeNames)
                //{
                types.Add(string.Concat(type, ".name"));
                //}
                types.Add(string.Concat(type, ".gameObject"));
                types.Add(string.Concat(type, ".hideFlags"));
                types.Add(string.Concat(type, ".mesh"));
                types.Add(string.Concat(type, ".material"));
                types.Add(string.Concat(type, ".materials"));
                types.Add(string.Concat(type, ".sharedMesh"));
                types.Add(string.Concat(type, ".sharedMaterial"));
                types.Add(string.Concat(type, ".sharedMaterials"));
                types.Add(string.Concat(type, ".isActiveAndEnabled"));

                if (type.IsSubclassOf(typeof(Renderer)))
                {
                    types.Add(string.Concat(type, ".isVisible"));
                    types.Add(string.Concat(type, ".bounds"));
                    types.Add(string.Concat(type, ".worldToLocalMatrix"));
                    types.Add(string.Concat(type, ".localToWorldMatrix"));
                }

                if (type.IsSubclassOf(typeof(Collider)))
                {
                    types.Add(string.Concat(type, ".bounds"));
                }
            }

            return types;
        }

        public static object GetValueNoError(this FieldInfo fieldInfo, object obj, bool verbose=false)
        {
            object value = default(object);

            try
            {
                value = fieldInfo.GetValue(obj);
            }
            catch (Exception exception)
            {
                if (verbose)
                {
                    Debug.Log("Couldnt access field " + obj.ToString() + "." + fieldInfo.Name + ". " + exception.Message);
                }
            }

            return value;
        }

        public static object GetValueNoError(this PropertyInfo propertyInfo, object obj, bool verbose=false)
        {
            object value = default(object);

            try
            {
                value = propertyInfo.GetValue(obj, null);
            }
            catch (Exception exception)
            {
                if (verbose)
                {
                    Debug.Log("Couldnt access field " + obj.ToString() + "." + propertyInfo.Name + ". " + exception.Message);
                }
            }

            return value;
        }

        public static List<PropertyInfo> GetPropertiesComparable(this Type type)
        {
            return GetPropertiesFiltered(type, type.CantCompare());
        }

        public static List<PropertyInfo> GetPropertiesCopyable(this Type type)
        {
            return GetPropertiesFiltered(type, type.CantCopy());
        }

        public static List<PropertyInfo> GetPropertiesFiltered(this Type type, List<string> typesToIgnore)
        {
            PropertyInfo[] propertiesUnfiltered = type.GetProperties();
            List<PropertyInfo> propertiesFiltered = new  List<PropertyInfo>{ };

            foreach (PropertyInfo property in propertiesUnfiltered)
            {
                string fullName = string.Concat(type.ToString(), ".", property.Name);

                if (!typesToIgnore.Contains(fullName))
                {
                    propertiesFiltered.Add(property);
                }
            }

            return propertiesFiltered;
        }

        public static void StartTempCoroutine(this MonoBehaviour monoBehaviour, IEnumerator iEnumerator, float endAfterSeconds)
        {
            Coroutine coroutine = monoBehaviour.StartCoroutine(iEnumerator);
            coroutine.EndAfter(endAfterSeconds, monoBehaviour);
        }

        public static void EndAfter(this Coroutine coroutine, float seconds, MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(StopCoroutineDelayed(coroutine, seconds, monoBehaviour));
        }

        static IEnumerator StopCoroutineDelayed(Coroutine coroutine, float delay, MonoBehaviour monoBehaviour)
        {
            yield return new WaitForSeconds(delay);
            monoBehaviour.StopCoroutine(coroutine);
        }

        public static void Set(this Animator animator, bool variable, string variableName)
        {
            animator.SetBool(variableName, variable);
        }

        public static void Set(this Animator animator, float variable, string variableName)
        {
            animator.SetFloat(variableName, variable);
        }

        public static void Set(this Animator animator, int variable, string variableName)
        {
            animator.SetInteger(variableName, variable);
        }

        public static void Set(this Animator animator, string triggerName)
        {
            animator.SetTrigger(triggerName);
        }

        public static bool IsStateActive(this Animator animator, string state)
        {
            for (int i = 0; i < animator.layerCount; i++)
            {
                if (animator.GetCurrentAnimatorStateInfo(i).IsName(state))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static AnimationClip GetLegacy(this GameObject target, string name)
        {
            Animation animation = target.GetComponent<Animation>();

            return animation.GetLegacy(name);
        }

        public static AnimationClip GetLegacy(this Animation animation, string name)
        {
            AnimationClip animationClip = null;
            if (animation != null)
            {
                animationClip = animation.GetClip(name);
            }
            
            if (animation == null || animationClip == null)
            {
                Debug.LogWarning("Play Legacy could not find the animation " + name);
            }

            return animationClip;
        }

        public static void GoToStart(this Animation animation, string name)
        {
            animation.GoToStart(animation.GetLegacy(name));
        }

        public static void GoToStart(this Animation animation, AnimationClip animationClip = null)
        {
            if (animationClip != null)
            {
                animation.clip = animationClip;
            }

            animation.Play();
            animation.Sample();
            animation.Stop();
        }

        public static Animation PlayLegacy(this MonoBehaviour target, String name, bool force = false, float delay = 0)
        {
            return target.gameObject.PlayLegacy(name, force, delay, target);
        }

        public static Animation PlayLegacy(this GameObject target, String name, bool force = false, float delay = 0, MonoBehaviour monoBehavior = null)
        {
            return PlayLegacy(target, target.GetLegacy(name), force, delay, monoBehavior);
        }

        public static Animation PlayLegacy(this MonoBehaviour target, AnimationClip animationClip, bool force = false, float delay = 0)
        {
            return target.gameObject.PlayLegacy(animationClip, force, delay, target);
        }

        public static Animation PlayLegacy(this GameObject target, AnimationClip animationClip, bool force = false, float delay = 0, MonoBehaviour monoBehavior = null)
        {
            Animation animation = AddLegacy(target, animationClip);

            if (delay > 0 && monoBehavior == null)
            {
                Debug.LogWarning("PlayLegacy was called with a dealy and no monobehavor. Animation will be played without a delay.");
            }
            
            if (monoBehavior != null && delay > 0)
            { 
                monoBehavior.StartCoroutine(PlayClipDelayed(animation, animationClip, force, delay));
            }
            else
            {
                PlayClip(animation, animationClip, force);
            }

            return animation;
        }

        public static Animation AddLegacy(this MonoBehaviour target, AnimationClip animationClip, string name = "")
        {
            return target.gameObject.AddLegacy(animationClip, name);
        }
        
        public static Animation AddLegacy(this GameObject target, AnimationClip animationClip, string name = "")
        {
            Animation animation = target.GetComponent<Animation>();
            if (animation == null)
            {
                animation = target.gameObject.AddComponent<Animation>();
            }

            if (name == "")
            {
                name = animationClip.name;
            }
            
            if (animation.GetClip(animationClip.name) == null)
            {
                animation.AddClip(animationClip, name);
                animation.clip = animationClip;
            }

            return animation;
        }

        private static IEnumerator PlayClipDelayed(Animation animation, AnimationClip animationClip, bool force = false, float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            PlayClip(animation, animationClip, force);
        }

        private static void PlayClip(Animation animation, AnimationClip animationClip, bool force)
        {
            if (!animation.isPlaying || force)
            {
                animation.Stop();
                animation.Play(animationClip.name);
            }
        }
        
        public static List<FieldInfo> GetFieldsComparable(this Type type)
        {
            return GetFieldsFiltered(type, type.CantCompare());
        }

        public static List<FieldInfo> GetFieldsCopyable(this Type type)
        {
            return GetFieldsFiltered(type, type.CantCopy());
        }

        public static List<FieldInfo> GetFieldsFiltered(this Type type, List<string> typesToIgnore)
        {
            FieldInfo[] fieldsUnfiltered = type.GetFields();
            List<FieldInfo> fieldsFiltered = new  List<FieldInfo>{ };

            foreach (FieldInfo property in fieldsUnfiltered)
            {
                string fullName = string.Concat(type.ToString(), ".", property.Name);

                if (!typesToIgnore.Contains(fullName))
                {
                    fieldsFiltered.Add(property);
                }
            }

            return fieldsFiltered;
        }
        
        public static Component CopyComponent<T>(this GameObject destination, T original, bool verbose = false) where T : Component
        {
            System.Type type = original.GetType();

            Component copy = null;

            if (Application.isPlaying)
            {
                copy = destination.AddComponent(type);
            }
            else
            {
                #if UNITY_EDITOR
                    copy = Undo.AddComponent(destination, type);
                #endif
            }

            if (copy == null)
            {
                Debug.Log(@"TinyExtensions.CopyComponent failed to copy component of type " + type + @". Sometimes this happens when 
                    Unity automatically adds components. Instead we will try to copy over the Unity added component.");

                copy = destination.GetComponent(type);
            }

            if (type == typeof(Transform))
            {
                Transform copyTransform = copy as Transform;
                Transform originalTransform = original as Transform;
                
                copyTransform.localScale = originalTransform.localScale;
                copyTransform.localRotation = originalTransform.localRotation;
                copyTransform.localPosition = originalTransform.localPosition;

                copy = copyTransform;
            }
            else
            {
                List<FieldInfo> fields = type.GetFieldsCopyable();
                foreach (FieldInfo field in fields)
                {
                    try
                    {
                        field.SetValue(copy, field.GetValue(original));
                    }
                    catch (Exception exception)
                    {
                        if (verbose)
                        {
                            Debug.Log("Couldnt set field " + type.ToString() + "." + field.Name + ". " + exception.Message);
                        }
                    }
                }

                List<PropertyInfo> properties = type.GetPropertiesCopyable();
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        property.SetValue(copy, property.GetValue(original, null), null);
                    }
                    catch (Exception exception)
                    {
                        if (verbose)
                        {
                            Debug.Log("Couldnt access property " + type.ToString() + "." + property.Name + ". " + exception.Message);
                        }
                    }
                }
            }

            return copy;
        }

        public static void SetOnlyListener(this UnityEvent unityEvent, UnityAction call)
        {
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(call);
        }
    }
}
