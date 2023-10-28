using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenMod.Unturned.Events;

internal abstract class UnturnedPlayerEventsListener : UnturnedEventsListener, IUnturnedPlayerEventsListener
{
    private record EventRegistration(
        Action<Player, Delegate> Subscribe,
        Action<Player, Delegate> Unsubscribe,
        Func<Player, Delegate> HandlerFactory
    );

    private record EventSubscription(
        Player Player,
        Delegate Handler,
        Action<Player, Delegate> Unsubscribe
    );

    private readonly List<EventRegistration> m_Registrations = new();
    private readonly List<EventSubscription> m_Subscriptions = new();

    protected UnturnedPlayerEventsListener(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public virtual void SubscribePlayer(Player player)
    {
        foreach (var registration in m_Registrations)
        {
            var handler = registration.HandlerFactory(player);
            registration.Subscribe(player, handler);

            var subscription = new EventSubscription(player, handler, registration.Unsubscribe);
            m_Subscriptions.Add(subscription);
        }
    }

    public virtual void UnsubscribePlayer(Player player)
    {
        for (var i = m_Subscriptions.Count - 1; i >= 0; i--)
        {
            var (subscribedPlayer, handler, unsubscribe) = m_Subscriptions[i];

            if (subscribedPlayer != player)
            {
                continue;
            }

            m_Subscriptions.RemoveAtFast(i);
            unsubscribe(player, handler);
        }
    }

    protected delegate ref T FieldRefAccessor<in TInstance, T>(TInstance obj) where TInstance : class;

    // Should be used on delegate fields
    protected void SubscribePlayer<TDelegate>(
        FieldRefAccessor<Player, TDelegate> delegateAccessor,
        Func<Player, TDelegate> handlerFactory)
        where TDelegate : Delegate
    {
        SubscribePlayer(SubscribeToDelegate, UnsubscribeFromDelegate, handlerFactory);
        return;

        void SubscribeToDelegate(Player player, TDelegate handler)
        {
            ref var @delegate = ref delegateAccessor(player);
            @delegate = (TDelegate) Delegate.Combine(@delegate, handler);
        }

        void UnsubscribeFromDelegate(Player player, TDelegate handler)
        {
            ref var @delegate = ref delegateAccessor(player);
            @delegate = (TDelegate) (Delegate.Remove(@delegate, handler) ?? @delegate);
        }
    }

    // Should be used on event fields
    protected void SubscribePlayer<TDelegate>(
        Action<Player, TDelegate> subscribe,
        Action<Player, TDelegate> unsubscribe,
        Func<Player, TDelegate> handlerFactory)
        where TDelegate : Delegate
    {
        // Upcast to non-generic types so they can be stored in a list
        var subscribeUpcast = Unsafe.As<Action<Player, Delegate>>(subscribe);
        var unsubscribeUpcast = Unsafe.As<Action<Player, Delegate>>(unsubscribe);
        var handlerFactoryUpcast = Unsafe.As<Func<Player, Delegate>>(handlerFactory);

        var registration = new EventRegistration(subscribeUpcast, unsubscribeUpcast, handlerFactoryUpcast);
        m_Registrations.Add(registration);
    }
}