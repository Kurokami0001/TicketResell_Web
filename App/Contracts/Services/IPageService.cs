﻿using System;

namespace App.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);
}