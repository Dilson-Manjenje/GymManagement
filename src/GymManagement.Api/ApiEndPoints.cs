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
        private const string _base = $"/Gyms";
        public const string Base = $"{_base}";
        public const string Create = $"{_base}";
        public const string Get = $"{_base}/{{id}}";
        public const string List = $"{_base}";
        public const string Update = $"{_base}/{{id:guid}}";
        public const string Delete = $"{_base}/{{id:guid}}";
        public const string Rooms = $"{_base}/{{gymId:guid}}/Rooms";
        public const string Trainers = $"{_base}/{{gymId:guid}}/Trainers";
        public const string Members = $"{_base}/{{gymId:guid}}/Members";
        public const string Sessions = $"{_base}/{{gymId:guid}}/Sessions";
        public const string Subscriptions = $"{_base}/{{gymId:guid}}/Subscriptions";
    }

    public static class Rooms
    {

        private const string _base = $"/Rooms";
        public const string Base = $"{_base}";
        public const string Create = $"{_base}";
        public const string Get = $"{_base}/{{id}}";
        public const string List = $"{_base}";
        public const string Update = $"{_base}/{{id:guid}}";
        public const string Delete = $"{_base}/{{id:guid}}";
        public const string Sessions = $"{_base}/{{roomId:guid}}/Sessions";
    }
}