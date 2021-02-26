using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Att.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XkgWiki.Data.Repositories;
using XkgWiki.Domain;

namespace XkgWiki.Data.Stores
{
    public class UserStore : UserStoreBase<User, Guid, Claim, UserLogin, UserToken>, IUserRoleStore<User>
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserTokenRepository _userTokenRepository;

        public override IQueryable<User> Users => _userRepository.QueryAll();

        public UserStore(IdentityErrorDescriber describer,
            IUserRepository userRepository,
            IUserLoginRepository userLoginRepository,
            IUserTokenRepository userTokenRepository,
            IClaimRepository claimRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IUnitOfWork unitOfWork) : base(describer)
        {
            _userRepository = userRepository;
            _userLoginRepository = userLoginRepository;
            _userTokenRepository = userTokenRepository;
            _claimRepository = claimRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        public override async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            _userRepository.Save(user);
            await _unitOfWork.CommitAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            _userRepository.Save(user);
            await _unitOfWork.CommitAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            _userRepository.Delete(user);
            await _unitOfWork.CommitAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public override async Task<User> FindByIdAsync(string userName, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            // userid is username
            return await _userRepository.GetByUsernameAsync(userName);
        }

        public override async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userRepository.GetByUsernameAsync(normalizedUserName);
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var role = await _roleRepository.SingleAsync(r => r.Name == roleName);

            _userRoleRepository.Save(new UserRole { RoleId = role.Id, UserId = user.Id });
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var userRole = await _userRoleRepository.SingleOrDefaultAsync(ur => ur.UserId == user.Id && ur.Role.Name == roleName);

            if (userRole == null)
                return;

            _userRoleRepository.Delete(userRole);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userRoleRepository.QueryMany(ur => ur.UserId == user.Id, ur => ur.Role.Name).ToListAsync(cancellationToken);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            throw new NotImplementedException();
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userRoleRepository.QueryMany(ur => ur.Role.Name == roleName, ur => ur.User).ToListAsync(cancellationToken);
        }

        protected override async Task<User> FindUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userRepository.GetByIdAsync(userId, false);
        }

        protected override async Task<UserLogin> FindUserLoginAsync(Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userLoginRepository.SingleAsync(ul => ul.Id == userId && ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey);
        }

        protected override async Task<UserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userLoginRepository.SingleAsync(ul => ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey);
        }

        public override async Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _claimRepository.QueryMany(c => c.UserId == user.Id, c => new System.Security.Claims.Claim(c.ClaimType, c.ClaimValue))
                .ToListAsync(cancellationToken);
        }

        public override async Task AddClaimsAsync(User user, IEnumerable<System.Security.Claims.Claim> claims,
            CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            foreach (var claim in claims)
            {
                _claimRepository.Save(new Claim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public override async Task ReplaceClaimAsync(User user, System.Security.Claims.Claim claimModel, System.Security.Claims.Claim newClaimModel,
            CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var claim = await _claimRepository.SingleAsync(c => c.UserId == user.Id && c.ClaimType == claimModel.Value && c.ClaimValue == claimModel.Value);

            claim.ClaimType = newClaimModel.Type;
            claim.ClaimValue = newClaimModel.Value;

            _claimRepository.Save(claim);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public override async Task RemoveClaimsAsync(User user, IEnumerable<System.Security.Claims.Claim> claims,
            CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            foreach (var claimModel in claims)
            {
                var claim = await _claimRepository.SingleAsync(c => c.UserId == user.Id && c.ClaimType == claimModel.Type && c.ClaimValue == claimModel.Value);
                _claimRepository.Delete(claim);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public override async Task<IList<User>> GetUsersForClaimAsync(System.Security.Claims.Claim claim,
            CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _claimRepository.QueryMany(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type, c => c.User).ToListAsync(cancellationToken);
        }

        protected override async Task<UserToken> FindTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userTokenRepository.SingleAsync(ut => ut.LoginProvider == loginProvider && ut.Name == name && ut.UserId == user.Id);
        }

        protected override async Task AddUserTokenAsync(UserToken token)
        {
            ThrowIfDisposed();

            _userTokenRepository.Save(token);
            await _unitOfWork.CommitAsync();
        }

        protected override async Task RemoveUserTokenAsync(UserToken token)
        {
            ThrowIfDisposed();

            _userTokenRepository.Delete(token);
            await _unitOfWork.CommitAsync();
        }

        public override Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            // TODO:
            throw new NotImplementedException();
        }

        public override async Task RemoveLoginAsync(User user, string loginProvider, string providerKey,
            CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var userLogin = await _userLoginRepository.SingleAsync(ul =>
                ul.LoginProvider == loginProvider && ul.ProviderKey == providerKey && ul.UserId == user.Id);

            _userLoginRepository.Delete(userLogin);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public override async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var result = await _userLoginRepository.GetManyAsync(ul => ul.UserId == user.Id,
                ul => new UserLoginInfo(ul.LoginProvider, ul.ProviderKey, ul.ProviderDisplayName));

            return result.ToList();
        }

        public override async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _userRepository.GetByEmailAsync(normalizedEmail);
        }
    }
}