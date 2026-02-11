namespace GymManagement.Contracts.Bookings;

public sealed record CreateBookingRequest(Guid SessionId,
                                          Guid MemberId);    
                                          
public sealed record UpdateBookingRequest(Guid Id,
                                          Guid SessionId,
                                          Guid MemberId);                                              