using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;
using DomainEventDemo.Services;
using DomainEventDemo.Services.DomainEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainEventDemo
{
    [TestClass]
    public class DomainEventDispatcherTests
    {
        [TestMethod]
        public async Task DomainEventDispatcher_Emits_DomainEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<DomainEvent>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<DomainEvent>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"DomainEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<DomainEvent>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new DomainEvent());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_EntityChangedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityChangedEvent>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityChangedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"DomainEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityChangedEvent>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityChangedEvent());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_Base_DomainEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<DomainEvent>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<DomainEvent>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"DomainEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<DomainEvent>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityAddedEvent<Parent>());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_Base_EntityChangedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityChangedEvent>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityChangedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"DomainEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityChangedEvent>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityChangedEvent<Parent>());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_ParentEntityChangedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityChangedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityChangedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentChangedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityChangedEvent<Parent>>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityAddedEvent<Parent>());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_ParentEntityAddedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityAddedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityAddedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentAddedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityAddedEvent<Parent>>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityAddedEvent<Parent>());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_ParentEntityUpdatedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityUpdatedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityUpdatedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentUpdatedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityUpdatedEvent<Parent>>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityUpdatedEvent<Parent>());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_ParentEntityDeletedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityDeletedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityDeletedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentDeletedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityDeletedEvent<Parent>>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityDeletedEvent<Parent>());

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Does_not_emit_ChildEntityDeletedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityDeletedEvent<Child>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityDeletedEvent<Child>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentDeletedEventHandlerMock called for {e.GetType().FullName}"))
                .Throws(new AmbiguousMatchException("Should not have called IHandle<EntityDeletedEvent<Child>>)"));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityDeletedEvent<Child>>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new EntityDeletedEvent<Parent>());

            // Assert
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Does_not_emit_EntityChangedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityChangedEvent>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityChangedEvent>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentDeletedEventHandlerMock called for {e.GetType().FullName}"))
                .Throws(new AmbiguousMatchException("Should not have called IHandle<EntityDeletedEvent<Child>>)"));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(It.Is<Type>(t => t == typeof(IEnumerable<IHandle<EntityChangedEvent>>))))
                .Returns(new[] {handlerMock.Object});
            
            var dispatcher = new DomainEventDispatcher(serviceProviderMock.Object, new ObjectCloner());

            // Act
            await dispatcher.DispatchAsync(new DomainEvent());

            // Assert
        }
    }
}
