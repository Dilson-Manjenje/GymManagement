using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymManagement.Domain.Sessions;

namespace GymManagement.Application.Sessions.Queries.Dtos;

public record SessionDto(Guid Id,
                                string Title,                                
                                string GymName,
                                Guid GymId,
                                Guid RoomId,
                                string RoomName,
                                Guid TrainerId,
                                string TrainerName,
                                int Capacity,
                                int Vacancy,
                                DateTime StartDate,
                                DateTime EndDate,
                                SessionStatus Status)
{
    public static SessionDto MapToDto(Session session)
    {
        return new SessionDto
        (
            Id: session.Id,
            Title: session.Title,
            GymName: session.Room.Gym.Name,
            GymId: session.Room.GymId,
            RoomId: session.RoomId,
            RoomName: session.Room.Name,
            TrainerId: session.TrainerId,
            TrainerName: session.Trainer.Name,
            Capacity: session.Capacity,
            Vacancy: session.Vacancy,
            StartDate: session.StartDate,
            EndDate: session.EndDate,
            Status: session.Status
        );
    }
}                                    