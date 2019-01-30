using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;
using DomainEventDemo.DataAccess;
using DomainEventDemo.Services;
using DomainEventDemo.Services.DomainEvents;
using DomainEventDemo.Services.DomainEvents.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DomainEventDemo
{
    [TestClass]
    public class DemoContextTests
    {
        private static IServiceProvider ConfigureServices(Action<IServiceCollection> customRegistraion = null)
        {
            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<DemoContext>(options => options.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB; Integrated Security=true ;Database=DomainEventDemo"))
                .Scan(scan => 
                    scan.FromAssemblyOf<DomainEventDispatcher>()
                        .AddClasses(c => c.NotInNamespaceOf<DomainEventHandler>())
                        .AsImplementedInterfaces()
                        .WithTransientLifetime()
                );

            customRegistraion?.Invoke(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        [ClassInitialize]
        public static async Task Before_Any_Test(TestContext testContext)
        {
            using (var scope = ConfigureServices().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
        }

        [TestMethod]
        public async Task DomainEventDispatcher_Emits_ParentEntityChangedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityChangedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityChangedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentEntityChangedEventHandlerMock called for {e.GetType().FullName}"))
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
        public async Task DomainEventDispatcher_Emits_ParentAddedEvent()
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
        public async Task Adding_A_Parent_Emits_EntityAddedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityAddedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityAddedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentAddedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();
            
            using (var scope = ConfigureServices(
                    collection => collection.AddScoped(x => handlerMock.Object))
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.BeginTransactionAsync();
                // Act
                context.Parents.Add(new Parent
                {
                    Name = "Test Parent",
                    Description = "Mom",
                    JobCount = 3
                });
                await context.SaveChangesAsync();
            }

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task Editing_A_Parent_Emits_ParentEntityUpdatedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityUpdatedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityUpdatedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentUpdatedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();
            
            using (var scope = ConfigureServices(
                    collection => collection.AddScoped(x => handlerMock.Object))
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.BeginTransactionAsync();
                context.Parents.Add(new Parent
                {
                    Name = "Test Parent",
                    Description = "Mom",
                    JobCount = 3
                });
                await context.SaveChangesAsync();

                var parent = await context.Parents.FirstAsync();

                // Act
                parent.Description = "Dad";
                parent.JobCount = 5;
                await context.SaveChangesAsync();
            }

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task Deleting_A_Parent_Emits_ParentEntityDeletedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityDeletedEvent<Parent>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityDeletedEvent<Parent>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ParentDeletedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();
            
            using (var scope = ConfigureServices(
                    collection => collection.AddScoped(x => handlerMock.Object))
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.BeginTransactionAsync();
                context.Parents.Add(new Parent
                {
                    Name = "Test Parent",
                    Description = "Mom",
                    JobCount = 3
                });
                await context.SaveChangesAsync();

                var parent = await context.Parents.FirstAsync();

                // Act
                context.Parents.Remove(parent);
                await context.SaveChangesAsync();
            }

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task Adding_A_Child_Emits_ChildEntityAddedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityAddedEvent<Child>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityAddedEvent<Child>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ChildAddedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();
            
            using (var scope = ConfigureServices(
                    collection => collection.AddScoped(x => handlerMock.Object))
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.BeginTransactionAsync();
                // Act
                context.Parents.Add(new Parent
                {
                    Name = "Test Parent",
                    Description = "Mom",
                    JobCount = 3,
                    Children = {
                        new Child
                        {
                            Description = "Boy",
                            FriendCount = 5,
                            Name = "Test Child"
                        }}
                });
                await context.SaveChangesAsync();
            }

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task Editing_A_Child_Emits_ChildEntityUpdatedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityUpdatedEvent<Child>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityUpdatedEvent<Child>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ChildUpdatedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();
            
            using (var scope = ConfigureServices(
                    collection => collection.AddScoped(x => handlerMock.Object))
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.BeginTransactionAsync();
                context.Parents.Add(new Parent
                {
                    Name = "Test Parent",
                    Description = "Mom",
                    JobCount = 3,
                    Children = {
                        new Child
                        {
                            Description = "Boy",
                            FriendCount = 5,
                            Name = "Test Child"
                        }}
                });
                await context.SaveChangesAsync();

                var child = await context.Parents.SelectMany(x => x.Children).FirstAsync();

                // Act
                child.Description = "Girl";
                child.GradeLevel = 5;
                await context.SaveChangesAsync();
            }

            // Assert
            handlerMock.Verify();
        }

        [TestMethod]
        public async Task Deleting_A_Child_Emits_ChildEntityDeletedEvent()
        {
            // Arrange
            var handlerMock = new Mock<IHandle<EntityDeletedEvent<Child>>>();
            handlerMock.Setup(x => x.HandleAsync(It.IsAny<EntityDeletedEvent<Child>>()))
                .Returns(Task.CompletedTask)
                .Callback((object e) => Console.WriteLine($"ChildDeletedEventHandlerMock called for {e.GetType().FullName}"))
                .Verifiable();
            
            using (var scope = ConfigureServices(
                    collection => collection.AddScoped(x => handlerMock.Object))
                .CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DemoContext>();
                await context.Database.BeginTransactionAsync();
                context.Parents.Add(new Parent
                {
                    Name = "Test Parent",
                    Description = "Mom",
                    JobCount = 3,
                    Children = {
                        new Child
                        {
                            Description = "Boy",
                            FriendCount = 5,
                            Name = "Test Child"
                        }}
                });
                await context.SaveChangesAsync();

                var child = await context.Parents.SelectMany(x => x.Children).FirstAsync();

                // Act
                context.Remove(child);
                await context.SaveChangesAsync();
            }

            // Assert
            handlerMock.Verify();
        }
    }
}
