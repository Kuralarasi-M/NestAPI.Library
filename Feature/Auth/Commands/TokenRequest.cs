using LibraryApp.Data;
using LibraryApp.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Feature.Auth.Commands
{
    public class TokenRequest:IRequest<TokenResponse>
    {
        public Guid userId { get; set; }
        public string fcmToken { get; set; }
    }
    public class TokenResponse
    {
        public int successCode { get; set; }
        public string Message { get; set; }
    }

    public class FCMTokenHandler : IRequestHandler<TokenRequest, TokenResponse>
    {
        private readonly AppDBContext context;
        public FCMTokenHandler(AppDBContext context)
        {
            this.context = context;
        }
        public async Task<TokenResponse> Handle(TokenRequest request,CancellationToken cancellationToken)
        {
            try
            {
                var existingToken = await context.UserFcmTokens.FirstOrDefaultAsync(S => S.FcmToken == request.fcmToken);
                if (existingToken != null)
                {
                    existingToken.UpdatedAt = DateTime.UtcNow;
                    existingToken.UserId = request.userId;
                }
                else
                {
                    var newToken = new UserFcmToken
                    {
                        UserId = request.userId,
                        FcmToken = request.fcmToken,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await context.AddAsync(newToken);
                }
                await context.SaveChangesAsync(cancellationToken);
                return new TokenResponse
                {
                    successCode = 200,
                    Message = "FCM Token Saved Successfully!."
                };
            }
            catch (Exception ex)
            {
                return new TokenResponse
                {
                    successCode = 500,
                    Message = ex.Message,
                };
            }
        }
    }

    public class RemoveTokenRequest : IRequest<TokenResponse>
    {
        public Guid UserId { get; set; }
        public string? FcmToken { get; set; }
    }

    public class RemoveTokenHandler : IRequestHandler<RemoveTokenRequest, TokenResponse>
    {
        private readonly AppDBContext _context;
        public RemoveTokenHandler(AppDBContext context)
        {
            _context = context;
        }

        public async Task<TokenResponse> Handle(RemoveTokenRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.UserFcmTokens.Where(t => t.UserId == request.UserId);
                if (!string.IsNullOrWhiteSpace(request.FcmToken))
                {
                    query = query.Where(t => t.FcmToken == request.FcmToken);
                }

                var tokensToRemove = await query.ToListAsync(cancellationToken);
                if (tokensToRemove.Count > 0)
                {
                    _context.UserFcmTokens.RemoveRange(tokensToRemove);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return new TokenResponse
                {
                    successCode = 200,
                    Message = "FCM Token(s) removed successfully."
                };
            }
            catch (Exception ex)
            {
                return new TokenResponse
                {
                    successCode = 500,
                    Message = ex.Message
                };
            }
        }
    }
}
