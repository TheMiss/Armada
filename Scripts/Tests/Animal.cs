using System;

namespace Armageddon.Tests
{
    [Serializable]
    public abstract class Animal
    {
        public override string ToString()
        {
            return GetType().Name;
        }
    }

    [Serializable]
    public class Dog : Animal
    {
    }

    [Serializable]
    public class Cat : Animal
    {
    }

    [Serializable]
    public class Horse : Animal
    {
    }

    [Serializable]
    public class Bird : Animal
    {
    }

    [Serializable]
    public class Lion : Animal
    {
    }
}
