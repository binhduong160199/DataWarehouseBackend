using DataWarehouse.API.Utils.Exceptions;
using HotChocolate.Resolvers;

namespace DataWarehouse.API.GraphQL;

public class GraphQlExceptionMiddleware
{
    private readonly FieldDelegate _next;

    public GraphQlExceptionMiddleware(FieldDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(IMiddlewareContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ForbiddenException ex)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetCode("FORBIDDEN")
                .Build());
        }
        catch (UnauthorizedException ex)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetCode("UNAUTHORIZED")
                .Build());
        }
        catch (NotFoundException ex)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetCode("NOT_FOUND")
                .Build());
        }
        catch (ValidationException ex)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetCode("VALIDATION_ERROR")
                .Build());
        }
        catch (Exception ex)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Internal Server Error: " + ex.Message)
                .SetCode("INTERNAL_ERROR")
                .Build());
        }
    }
}