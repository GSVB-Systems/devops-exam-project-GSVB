using System.Net.Mail;
using DevOpsAppContracts.Models;
using DevOpsAppRepo.Interfaces;
using DevOpsAppService.Rules.Interfaces;
using Microsoft.EntityFrameworkCore;
using service.Exceptions;
using Sieve.Models;

namespace DevOpsAppService.Rules
{
    public class UserRules : IUserRules
    {
        private readonly IUserRepository _repo;
        
        public UserRules(IUserRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        
        public async Task ValidateCreateAsync(CreateUserDto createDto)
        {
            if (createDto == null) throw new InvalidRequestException("RegistrationDTO skal medtages.");

            if (string.IsNullOrWhiteSpace(createDto.Username))
                throw new InvalidRequestException("Angiv et brugernavn.");

            if (string.IsNullOrWhiteSpace(createDto.Email))
                throw new InvalidRequestException("Angiv Email adresse.");

            if (!IsValidEmail(createDto.Email))
                throw new RangeValidationException("Angiv en gyldig Email adresse.");

            if (string.IsNullOrWhiteSpace(createDto.Password))
                throw new InvalidRequestException("Angiv et password.");

            var emailInUse = await _repo.AsQueryable()
                .AnyAsync(u => u.Email == createDto.Email);
            if (emailInUse)
                throw new DuplicateResourceException($"Emailen '{createDto.Email}' findes allerede i systemet.");
            
        }

        public async Task ValidateUpdateAsync(string id, UpdateUserDto updateDto)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidRequestException("Bruger ID skal medtages.");
            if (updateDto == null)
                throw new InvalidRequestException("UpdateUserDto skal medtages.");

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null)
                throw new ResourceNotFoundException($"Bruger med ID '{id}' findes ikke.");

            if (updateDto.Username is not null && string.IsNullOrWhiteSpace(updateDto.Username))
                throw new InvalidRequestException("Angiv et brugernavn.");

            if (updateDto.Email is not null)
            {
                if (string.IsNullOrWhiteSpace(updateDto.Email))
                    throw new InvalidRequestException("Angiv Email adresse.");

                if (!IsValidEmail(updateDto.Email))
                    throw new RangeValidationException("Angiv en gyldig Email adresse.");

                var emailInUse = await _repo.AsQueryable()
                    .AnyAsync(u => u.Email == updateDto.Email && u.UserId != id);
                if (emailInUse)
                    throw new DuplicateResourceException($"Emailen '{updateDto.Email}' findes allerede i systemet.");
            }

            if (updateDto.Password is not null && string.IsNullOrWhiteSpace(updateDto.Password))
                throw new InvalidRequestException("Angiv et password.");
        }

        public async Task ValidateDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidRequestException("Bruger ID skal medtages.");

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null)
                throw new ResourceNotFoundException($"Bruger med ID '{id}' findes ikke.");
        }

        public async Task ValidateGetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidRequestException("Bruger ID skal medtages.");

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null)
                throw new ResourceNotFoundException($"Bruger med ID '{id}' findes ikke.");
        }

        public Task ValidateGetAllAsync(SieveModel? parameters)
        {
            if (parameters?.Page is not null && parameters.Page <= 0)
                throw new RangeValidationException("Page skal være mere end 0.");

            if (parameters?.PageSize is not null && parameters.PageSize <= 0)
                throw new RangeValidationException("Side størrelse skal være mere end 0.");

            return Task.CompletedTask;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}