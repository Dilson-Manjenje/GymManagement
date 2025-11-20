using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace GymManagement.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request,
                                  RequestHandlerDelegate<TResponse> next,
                                  CancellationToken cancellationToken)
    {
        if (_validator is null)
            return await next(cancellationToken);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
            return await next(cancellationToken);

        // var errors = validationResult.Errors
        //             .Select(error => Error.Validation(
        //                 code: error.PropertyName,
        //                 description: error.ErrorMessage))
        //             .ToList();

        var errors = validationResult.Errors
                   .ConvertAll(error => Error.Validation(
                       code: error.PropertyName,
                       description: error.ErrorMessage))
                   .ToList();

        //return (TResponse) Errors.ValidationErrors(errors);
        return (dynamic) errors;
    }
}