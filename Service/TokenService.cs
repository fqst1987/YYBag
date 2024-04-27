using Microsoft.EntityFrameworkCore;
using YYBagProgram.Data;
using YYBagProgram.Models.Token;

namespace YYBagProgram.Service
{
    public class TokenService
    {
        private readonly YYBagProgramContext _context;

        public TokenService(YYBagProgramContext context)
        {
            _context = context;
        }

        public async Task<bool> isTokenExpire(string token)
        {
            var tokenRecord = await _context.Tokens.Where(t => t.Token.Equals(token)).FirstOrDefaultAsync();

            if (tokenRecord != null && tokenRecord.ExpirationTime > DateTime.Now)
            {
                return true; 
            }

            return false;
        }

        public async Task<bool> isTokenTaken(string token)
        {
            return await _context.Tokens.AnyAsync(row => row.Token.Equals(token));
        }

        public async Task AddToken(string token, DateTime expirationTime)
        {
            var tokenRecord = new Tokens { Token = token, ExpirationTime = expirationTime };
            _context.Tokens.Add(tokenRecord);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveExpiredTokens()
        {
            var now = DateTime.Now;
            var expiredTokens = await _context.Tokens.Where(t => t.ExpirationTime <= now).ToListAsync();

            if (expiredTokens.Any())
            {
                _context.Tokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveUsedTokens(string token)
        {
            var UsedTokens = await _context.Tokens.Where(t => t.Token.Equals(token)).FirstOrDefaultAsync();

            _context.Tokens.Remove(UsedTokens);
            await _context.SaveChangesAsync();
            var now = DateTime.Now;
        }
    }
}
