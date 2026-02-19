using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagement.Contracts.Common;

public abstract record class ListResponse<T>(IEnumerable<T>? Data = null);

// public abstract class ListResponse<T>(IEnumerable<T>? data = null)
// {
//     public IEnumerable<T>? Data { get; set; } = data;
// }

