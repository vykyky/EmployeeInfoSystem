using EmployeeInfoSystem.Application.Interfaces;
using EmployeeInfoSystem.Application.Interfaces.Services;
using EmployeeInfoSystem.Domain;
using EmployeeInfoSystem.Infrastructure.External;
using Microsoft.EntityFrameworkCore;

namespace EmployeeInfoSystem.Infrastructure.Services
{
    public class SyncService : ISyncService
    {
        private readonly IUnitOfWork _uow;
        private readonly GalaktikaDbContext _galaktika;

        public SyncService(IUnitOfWork uow, GalaktikaDbContext galaktika)
        {
            _uow = uow;
            _galaktika = galaktika;
        }

        public async Task<bool> TabnExistsAsync(string tabn)
            => await _galaktika.Lschet.AnyAsync(l => l.Tabn == tabn);

        // ── Полная синхронизация (планировщик) ────────────────────────────────

        public async Task SyncAllAsync()
        {
            var tabns = await _galaktika.Lschet.Select(l => l.Tabn).ToListAsync();
            foreach (var tabn in tabns)
            {
                var user = await _uow.Users.GetByTabnAsync(tabn);
                if (user is null) continue;

                await SyncEmployeeByTabnAsync(tabn);
            }
        }

        public async Task SyncEmployeeByTabnAsync(string tabn)
        {
            await SyncProfileCoreAsync(tabn);
            await SyncPpeCoreAsync(tabn);
            await _uow.SaveChangesAsync();
        }

        // ── Раздельная синхронизация (админка) ────────────────────────────────

        public async Task SyncAllProfilesAsync()
        {
            var tabns = await _galaktika.Lschet.Select(l => l.Tabn).ToListAsync();
            foreach (var tabn in tabns)
            {
                var user = await _uow.Users.GetByTabnAsync(tabn);
                if (user is null) continue;

                await SyncProfileCoreAsync(tabn);
                await _uow.SaveChangesAsync();
            }
        }

        public async Task SyncAllPpeAsync()
        {
            var tabns = await _galaktika.Lschet.Select(l => l.Tabn).ToListAsync();
            foreach (var tabn in tabns)
            {
                // Пропускаем тех, у кого нет аккаунта в локальной БД —
                // внешний ключ ppecache.tabn ссылается на users.tabn
                var user = await _uow.Users.GetByTabnAsync(tabn);
                if (user is null) continue;

                await SyncPpeCoreAsync(tabn);
                await _uow.SaveChangesAsync();
            }
        }

        public async Task SyncProfileByTabnAsync(string tabn)
        {
            await SyncProfileCoreAsync(tabn);
            await _uow.SaveChangesAsync();
        }

        public async Task SyncPpeByTabnAsync(string tabn)
        {
            await SyncPpeCoreAsync(tabn);
            await _uow.SaveChangesAsync();
        }

        // ── Приватные реализации ───────────────────────────────────────────────

        private async Task SyncProfileCoreAsync(string tabn)
        {
            var data = await (
                from l in _galaktika.Lschet
                join p in _galaktika.Persons on l.TPerson equals p.Nrec
                join pc in _galaktika.PersCard on l.Nrec equals pc.CLschet into pcJoin
                from pc in pcJoin.DefaultIfEmpty()
                where l.Tabn == tabn
                select new { l, p, pc }
            ).FirstOrDefaultAsync();

            if (data == null) return;

            var sizes = data.pc?.Sizes;
            var existing = await _uow.EmployeeProfiles.GetByTabnAsync(tabn);

            if (existing == null)
            {
                await _uow.EmployeeProfiles.AddAsync(new EmployeeProfile
                {
                    Tabn = tabn,
                    Fio = data.p.Fio,
                    BornDate = data.p.BornDate,
                    HireDate = data.l.DatPos,
                    Phone = data.p.Phone,
                    Email = data.p.Email,
                    Height = sizes?.ElementAtOrDefault(0),
                    ClothesSize = sizes?.ElementAtOrDefault(1),
                    WinterClothesSize = sizes?.ElementAtOrDefault(2),
                    ShoesSize = sizes?.ElementAtOrDefault(3),
                    WinterShoesSize = sizes?.ElementAtOrDefault(8),
                    SyncedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Fio = data.p.Fio;
                existing.BornDate = data.p.BornDate;
                existing.HireDate = data.l.DatPos;
                existing.Phone = data.p.Phone;
                existing.Email = data.p.Email;
                existing.Height = sizes?.ElementAtOrDefault(0);
                existing.ClothesSize = sizes?.ElementAtOrDefault(1);
                existing.WinterClothesSize = sizes?.ElementAtOrDefault(2);
                existing.ShoesSize = sizes?.ElementAtOrDefault(3);
                existing.WinterShoesSize = sizes?.ElementAtOrDefault(8);
                existing.SyncedAt = DateTime.UtcNow;
                await _uow.EmployeeProfiles.UpdateAsync(existing);
            }
        }

        private async Task SyncPpeCoreAsync(string tabn)
        {
            var data = await (
                from l in _galaktika.Lschet
                join pc in _galaktika.PersCard on l.Nrec equals pc.CLschet
                join ps in _galaktika.PersSfo on pc.Nrec equals ps.CPerscard
                join g in _galaktika.GroupSfo on ps.CGrupSfo equals g.Nrec
                join k in _galaktika.KatMbp on ps.CKatMbp equals k.Nrec
                where l.Tabn == tabn && ps.Spisdate == 0
                select new
                {
                    GroupName = g.Name,
                    ItemName = k.Name,
                    ps.GiveDate,
                    ps.EndDate,
                    ps.Srok,
                    ps.CurKol
                }
            ).ToListAsync();

            await _uow.Ppes.DeleteByTabnAsync(tabn);

            foreach (var item in data)
            {
                await _uow.Ppes.AddAsync(new Ppe
                {
                    Tabn = tabn,
                    GroupName = item.GroupName,
                    ItemName = item.ItemName,
                    GiveDate = item.GiveDate,
                    EndDate = item.EndDate,
                    WearPeriod = item.Srok,
                    Quantity = item.CurKol,
                    SyncedAt = DateTime.UtcNow
                });
            }
        }
    }
}