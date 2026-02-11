using GymManagement.Contracts.Common;

namespace GymManagement.Contracts.Bookings;

public record BookingResponse(Guid Id,
                              string Title,
                              Guid MemberId,
                              string MemberName,
                              Guid RoomId,
                              string RoomName,
                              Guid TrainerId,
                              string TrainerName,
                              DateTime StartDate,
                              DateTime EndDate,
                              BookingStatusType Status,
                              Guid SessionId);

public record ListBookingResponse(IEnumerable<BookingResponse> Data) : ListResponse<BookingResponse>(Data: Data);                                                                             