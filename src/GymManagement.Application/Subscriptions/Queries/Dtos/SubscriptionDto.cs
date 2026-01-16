using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Subscriptions;

namespace GymManagement.Application.Subscriptions.Queries.Dtos;

public record SubscriptionDto(Guid Id,
                              SubscriptionType SubscriptionType,
                              decimal Price,
                              DateTime StartDate,
                              DateTime EndDate,
                              bool IsActive,
                              string GymName,
                              int MaxRooms,
                              //List<string> Rooms,
                              int MaxDailySessions,
                              Guid MemberId,
                              string UserName)
{
    public static SubscriptionDto MapToDto(Subscription subscription)
    {
        string gymName = subscription.Member?.Gym?.Name ?? "";
        string UserName = subscription.Member?.UserName ?? "";

        return new SubscriptionDto(
            Id: subscription.Id,
            SubscriptionType: subscription.SubscriptionType,
            Price: subscription.Price,
            StartDate: subscription.StartDate,
            EndDate: subscription.EndDate,
            IsActive: subscription.IsActive,
            GymName: gymName,
            MaxRooms: subscription.SubscriptionType.MaxRooms,
            MaxDailySessions: subscription.SubscriptionType.MaxDailySessions,
            MemberId: subscription.MemberId,
            UserName: UserName
        );

    }
}

