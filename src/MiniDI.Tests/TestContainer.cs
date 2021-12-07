using System;
using NUnit.Framework;

namespace MiniDI.Tests;

public class TestContainer
{
    [Test]
    public void ShouldBeAbleToResolveASingleDependency()
    {
        // Arrange
        var container = new Container();
        container.RegisterTransient<ICar, Car>();
        container.RegisterTransient<IEngine, ElectricEngine>();

        // Act
        var result = container.Resolve<ICar>();
        // Assert
        result.Accelerate();
    }

    public interface ICar
    {
        public void Accelerate();
    }

    public interface IEngine
    {
        public string OpenThrottle();
    }

    public class Car : ICar
    {
        private readonly IEngine _engine;

        public Car(IEngine engine)
        {
            _engine = engine;
        }

        public void Accelerate()
        {
            Console.WriteLine(_engine.OpenThrottle());
        }
    }

    public class PetrolEngine : IEngine
    {
        public string OpenThrottle()
        {
            return "vroooom!";
        }
    }

    public class ElectricEngine : IEngine
    {
        public string OpenThrottle()
        {
            return "bzzzzt";
        }
    }

}