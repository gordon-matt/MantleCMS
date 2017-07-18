﻿using System.Threading.Tasks;

namespace Mantle.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}