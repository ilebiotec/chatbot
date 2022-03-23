using CamBot.Common.Models.MedicalAppointment;
using CamBot.Common.Models.Qualification;
using CamBot.Common.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CamBot.Data
{
    public interface IDataBaseService
    {
        DbSet<UserModel> User { get; set; }

        DbSet<QualificationModel> Qualification { get; set; }

        DbSet<MedicalAppointmentModel> MedicalAppointment { get; set; }

        Task<bool> SaveAsync();
    }
}
