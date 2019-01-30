using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DomainEventDemo.Services.DomainEvents
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IObjectCloner _cloner;

        public DomainEventDispatcher(IServiceProvider serviceProvider, IObjectCloner cloner)
        {
            _serviceProvider = serviceProvider;
            _cloner = cloner;
        }

        public async Task DispatchAsync(DomainEvent domainEvent)
        {
            try
            {
                var dispatchMethod = GetType().GetMethod(nameof(DispatchTypeAsync), BindingFlags.Instance | BindingFlags.NonPublic);
                if (dispatchMethod == null)
                {
                    return;
                }

                var baseDomainEventType = typeof(DomainEvent);
                var domainEventType = domainEvent.GetType();
                var allTypes = new List<Type> {domainEventType};
                while (domainEventType.BaseType != null && baseDomainEventType.IsAssignableFrom(domainEventType))
                {
                    domainEventType = domainEventType.BaseType;
                    allTypes.Add(domainEventType);
                }

                var allTasks = allTypes.Select(x =>
                    (Task) dispatchMethod.MakeGenericMethod(x).Invoke(this, new object[] {domainEvent})).ToArray();
                await Task.WhenAll(allTasks);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DomainMessageHError: Error getting handlers for {0} : {1}",
                    domainEvent.GetType().FullName,
                    ex);
            }
        }

        private async Task DispatchTypeAsync<T>(T domainEvent) where T : DomainEvent
        {
            try
            {
                var handlers = _serviceProvider.GetServices<IHandle<T>>() ?? Enumerable.Empty<IHandle<T>>();

                var eventTasks = handlers.Select(async handler =>
                {
                    try
                    {
                        var clonedEvent = domainEvent;
                        try
                        {
                            clonedEvent = _cloner.Clone(domainEvent);
                        }
                        catch { }

                        await handler.HandleAsync(clonedEvent).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("DomainMessageHandlerError: Error handling {0} using {1} : {2}", 
                            domainEvent.GetType().FullName, 
                            handler.GetType().FullName,
                            ex);
                    }
                }).ToArray();
                await Task.WhenAll(eventTasks);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DomainMessageHError: Error getting handlers for {0} : {1}",
                    domainEvent.GetType().FullName,
                    ex);
            }
        }
    }
}