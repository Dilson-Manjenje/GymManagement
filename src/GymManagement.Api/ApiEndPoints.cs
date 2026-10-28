using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagement.Api;

public static class ApiEndPoints
{
    private const string _apiBase = "api";

    public static class Gyms
    {
        //private const string _base = $"{_apiBase}/Gyms";
        public const string Base = "/Gyms";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Rooms = $"{Base}/{{gymId:guid}}/Rooms";
        public const string Trainers = $"{Base}/{{gymId:guid}}/Trainers";
        public const string Members = $"{Base}/{{gymId:guid}}/Members";
        public const string Sessions = $"{Base}/{{gymId:guid}}/Sessions";
        public const string Subscriptions = $"{Base}/{{gymId:guid}}/Subscriptions";
    }

    public static class Rooms
    {        
        public const string Base = $"/Rooms";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Sessions = $"{Base}/{{roomId:guid}}/Sessions";
    }

    public static class Members
    {
        public const string Base = $"/Members";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Subscriptions = $"{Base}/{{memberId:guid}}/Subscriptions";
        public const string Bookings = $"{Base}/{{memberId:guid}}/Bookings";
    }

    public static class Trainers
    {
        public const string Base = $"/Trainers";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Sessions = $"{Base}/{{trainerId:guid}}/Sessions";
    }

    public static class Subscriptions
    {
        public const string Base = $"/Subscriptions";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Disable = $"{Base}/{{id:guid}}/Disable";
        public const string Rooms = $"{Base}/{{subscriptionId:guid}}/rooms";
    }

    public static class Sessions
    {
        public const string Base = $"/Sessions";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
        public const string Bookings = $"{Base}/{{sessionId:guid}}/Bookings";
        public const string Cancel = $"{Base}/{{sessionId:guid}}/Cancel";
        public const string Finalize = $"{Base}/{{sessionId:guid}}/Finalize";
        public const string UpCommingSessions = $"{Base}/UpCommingSessions/{{gymId:guid}}";
    }
    
    public static class Bookings
    {
        public const string Base = $"/Bookings";
        public const string Create = $"{Base}";
        public const string Get = $"{Base}/{{id}}";
        public const string List = $"{Base}";
        public const string Cancel = $"{Base}/{{bookingId:guid}}/Cancel";
    }
}